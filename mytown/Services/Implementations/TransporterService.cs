using mytown.DataAccess.Interfaces;
using mytown.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using Stripe;
using System.Text;
using System.Text.Json;

namespace mytown.Services.Implementations
{
    public class TransporterService : ITransporterService
    {
        private readonly ITransporterRepository _repo;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<TransporterService> _logger;
        private readonly IVerificationLinkBuildertransporter _verificationLinkBuilder;
        private readonly HttpClient _httpClient;

        public TransporterService(
            ITransporterRepository repo,
            IEmailService emailService,
            IConfiguration configuration,
            ILogger<TransporterService> logger,
            IVerificationLinkBuildertransporter verificationLinkBuilder, HttpClient httpClient
            )
        {
            _repo = repo;
            _emailService = emailService;
            _configuration = configuration;
            _logger = logger;
            _verificationLinkBuilder = verificationLinkBuilder;
            _httpClient = httpClient;
        }

        // ---------------- REGISTER ----------------
        public async Task<(bool success, string message)> RegisterTransporterAsync(TransporterRegisterDto dto)
        {
            var (isTaken, statusMessage) = await _repo.IsEmailTaken(dto.Email);

            if (statusMessage != null)
                return (false, statusMessage);

            if (isTaken)
                return (false, "This email is already registered.");

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            var pending = new PendingTransporterVerification
            {
                Email = dto.Email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = JsonSerializer.Serialize(dto)
            };

            await _repo.SavePendingVerification(pending);
            await _emailService.SendVerificationEmail(dto.Email, link);

            return (true, "Verification email sent.");
        }

        // ---------------- VERIFY EMAIL ----------------
        public async Task<(bool success, string message, int? transporterRegId)> VerifyEmailAsync(string token)
        {
            var pending = await _repo.FindPendingVerificationByToken(token);

            if (pending == null || pending.ExpiryDate < DateTime.UtcNow)
                return (false, "Invalid or expired verification link.", null);

            var dto = JsonSerializer.Deserialize<TransporterRegisterDto>(pending.JsonPayload);

            var transporter = new TransporterRegister
            {
                TransporterName = dto.TransporterName,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password.Trim()),
                Address = dto.Address,
                Town = dto.Town,
                City = dto.City,
                State = dto.State,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                PhoneNumber = dto.PhoneNumber,
                IsEmailVerified = true,
                Status = "Active",
                TransporeterRegDate = DateTime.UtcNow
            };

            await _repo.RegisterTransporter(transporter);

            // Save bank account details
            var accountDetails = new TransporterAccountDetail
            {
                TransporterRegId = transporter.TransporterRegId,
                AccountHolderName = dto.AccountHolderName,
                BankName = dto.BankName,
                AccountNumber = dto.AccountNumber,
                IFSCCode = dto.IFSCCode,
                IsTermsAccepted = dto.IsTermsAccepted,
                CreatedDate = DateTime.UtcNow
            };

            await _repo.AddTransporterAccountDetails(accountDetails);

            // --- Cashfree beneficiary creation (non-blocking) ---
            bool payoutReady = false;
            try
            {
                var beneficiaryRequest = new CreateCashfreeBeneficiaryRequestTrans
                {
                    TransRegId = transporter.TransporterRegId,
                    BeneficiaryName = accountDetails.AccountHolderName,
                    BankAccountNumber = accountDetails.AccountNumber,
                    BankIfsc = accountDetails.IFSCCode
                };

                await CreateBeneficiaryAsync(beneficiaryRequest);
                payoutReady = true;
            }
            catch (Exception cfEx)
            {
                _logger.LogError(cfEx,
                    "Cashfree beneficiary creation failed for TransporterRegId {TransporterRegId}. Will retry later.",
                    transporter.TransporterRegId);
            }
            // --- end beneficiary block ---

            await _repo.DeletePendingVerification(token);

            return (true, "Email verified successfully.", transporter.TransporterRegId);
        }

        // ---------------- RESEND EMAIL ----------------
        public async Task<(bool success, string message)> ResendVerificationEmailAsync(string email)
        {
            var existing = await _repo.FindPendingVerificationByEmail(email);

            if (existing == null)
                return (false, "No pending verification found.");

            await _repo.DeletePendingVerification(existing.Token);  // was: RemoveVerification(existing)

            string token = Guid.NewGuid().ToString();
            DateTime expiry = DateTime.UtcNow.AddHours(24);

            var pending = new PendingTransporterVerification
            {
                Email = email,
                Token = token,
                ExpiryDate = expiry,
                JsonPayload = existing.JsonPayload  // was: missing — crashes VerifyEmailAsync without this
            };

            await _repo.SavePendingVerification(pending);

            string frontendBaseUrl = _configuration["FrontendBaseUrl"];
            string link = _verificationLinkBuilder.BuildLink(frontendBaseUrl, token);

            await _emailService.SendVerificationEmail(email, link);

            return (true, "Verification email resent.");
        }

        public async Task<CashfreeBeneficiaryResponse> CreateBeneficiaryAsync(
      CreateCashfreeBeneficiaryRequestTrans request)
        {
            var clientId = _configuration["CashfreePayout:ClientId"];
            var clientSecret = _configuration["CashfreePayout:ClientSecret"];
            var baseUrl = _configuration["Cashfree:BaseUrl"];
            // Falls back to clientId if CashfreePayout:SignatureClientId isn't set -
            
            var signatureClientId = _configuration["CashfreePayout:SignatureClientId"] ?? clientId;

            if (string.IsNullOrWhiteSpace(request.BeneficiaryId))
            {
                request.BeneficiaryId = $"TRANS_{request.TransRegId}";
            }

            var payload = new
            {
                beneficiary_id = request.BeneficiaryId,
                beneficiary_name = request.BeneficiaryName,
                beneficiary_instrument_details = new
                {
                    bank_account_number = request.BankAccountNumber,
                    bank_ifsc = request.BankIfsc
                },
                beneficiary_contact_details = new
                {
                    beneficiary_email = request.BeneficiaryEmail,
                    beneficiary_phone = request.BeneficiaryPhone,
                    beneficiary_country_code = request.BeneficiaryCountryCode
                }
            };

            var json = JsonSerializer.Serialize(payload);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}/beneficiary");
            httpRequest.Headers.Add("x-client-id", clientId);
            httpRequest.Headers.Add("x-client-secret", clientSecret);
            httpRequest.Headers.Add("x-api-version", "2024-01-01");
            httpRequest.Headers.Add("x-request-id", Guid.NewGuid().ToString());
            httpRequest.Headers.Add(
    "x-cf-signature",
    CashfreeSignatureHelper.GenerateSignature(signatureClientId, _configuration["CashfreePayoutPublicKey"]));
            httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Creating beneficiary failed. Status: {response.StatusCode}, Response: {responseContent}");
            }

            var cfResponse = JsonSerializer.Deserialize<CashfreeBeneficiaryResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            // --- save to DB ---
            var accountDetail = await _repo.GetTransporterAccountDetailByRegId(request.TransRegId);
            if (accountDetail != null)
            {
                accountDetail.CashfreeBeneficiaryId = cfResponse.BeneficiaryId;
                accountDetail.CashfreeBeneficiaryStatus = cfResponse.BeneficiaryStatus;
                accountDetail.CashfreeBeneficiaryCreatedDate = DateTime.UtcNow;

                await _repo.UpdateTransporterAccountDetails(accountDetail);
            }
            else
            {
                _logger.LogWarning(
                    "Beneficiary created on Cashfree but no TransporterAccountDetail found for TransRegId {TransRegId}",
                    request.TransRegId);
            }

            return cfResponse;
        }
    }
}

