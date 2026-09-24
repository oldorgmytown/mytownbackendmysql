using Microsoft.AspNetCore.Mvc;
using mytown.DataAccess;
using mytown.DataAccess.Interfaces;
using mytown.DataAccess.Repositories;
using mytown.Helpers;
using mytown.Models;
using mytown.Models.DTO_s;
using mytown.Services.Interfaces;
using MyTown.Models;
using Stripe;
using System.Net;
using System.Text;
using System.Text.Json;

namespace mytown.Services
{
    public class BusinessService : IBusinessService
    {
        private readonly IBusinessRepository _repo;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BusinessService> _logger;

        public BusinessService(IBusinessRepository repo, HttpClient httpClient, IConfiguration configuration, ILogger<BusinessService> logger)
        {
            _repo = repo;
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public Task<bool> IsEmailTaken(string email)
        {
            return _repo.IsEmailTaken(email);
        }

        public Task<PendingBusinessVerification?> FindPendingVerificationByToken(string token)
        {
            return _repo.FindPendingVerificationByToken(token);
        }

        public async Task<PendingBusinessVerification?> FindPendingVerificationByEmail(string email)
        {
            var verification = await _repo.FindPendingVerificationByEmail(email);
            return verification;
        }



        public Task SavePendingVerification(PendingBusinessVerification pending)
        {
            return _repo.SavePendingVerification(pending);
        }


        //public Task RemoveVerification(BusinessVerification verification)
        //{
        //    return _repo.RemoveVerification(verification);
        //}

        public Task DeletePendingVerification(string token)
        {
            return _repo.DeletePendingVerification(token);
        }

        public Task RegisterBusiness(BusinessRegister newBusiness)
        {
            return _repo.RegisterBusiness(newBusiness);
        }

        public Task CreateProfile(BusinessProfile profile)
        {
            return _repo.CreateProfile(profile);
        }

        public Task<BusinessRegister?> GetBusinessByIdAsync(int busRegId)
        {
            return _repo.GetBusinessByIdAsync(busRegId);
        }

        public async Task<IEnumerable<BusinessCategory>> GetBusinessCategories()
        {
            var result = await _repo.GetBusinessCategories();
            return result.Value ?? Enumerable.Empty<BusinessCategory>();
        }

        public Task<IEnumerable<ProductSubCategory>> BusinessSubCategoriesforStores(int buscatid)
        {
            return _repo.BusinessSubCategoriesforStores(buscatid);
        }

        public async Task<IEnumerable<ProductGroupResponseDto>> GetProductGroupsBySubCategoryId(int prodSubcatId)
        {
            return await _repo.GetProductGroupsBySubCategoryId(prodSubcatId);
        }

        public async Task<IEnumerable<ProductType>> GetProductTypesByGroupAndSubCategory(int prodSubcatId, int prodGroupId)
        {
            return await _repo.GetProductTypesByGroupAndSubCategory(prodSubcatId, prodGroupId);            
        }

        public async Task<IEnumerable<ProductAttributeDto>> GetAttributesBySubCategoryId(int prodSubcatId, int busCatId, int productGroupId)
        {
            return await _repo.GetAttributesBySubCategoryId(prodSubcatId, busCatId, productGroupId);
        }

       
        //add bank account details
        public async Task SaveBusinessAccountDetails(BusinessAccountDetail businessAccountDetail)
        {
            await _repo.SaveBusinessAccountDetails(businessAccountDetail);
        }
        public async Task<BankVerificationResponseDto>
      VerifyBankAccountAsync(
          BankVerificationRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.AccountNumber))
            {
                return new BankVerificationResponseDto
                {
                    Success = false,
                    Message = "Account number is required."
                };
            }

            if (string.IsNullOrWhiteSpace(request.Ifsc))
            {
                return new BankVerificationResponseDto
                {
                    Success = false,
                    Message = "IFSC code is required."
                };
            }

            request.AccountNumber =
                request.AccountNumber.Trim();

            request.Ifsc =
                request.Ifsc.Trim().ToUpper();

            return await _repo
                .VerifyBankAccountAsync(request);
        }

        public async Task<CashfreeBeneficiaryResponse> CreateBeneficiaryAsync(
          CreateCashfreeBeneficiaryRequest request)
        {
            var clientId = _configuration["CashfreePayout:ClientId"];
            var clientSecret = _configuration["CashfreePayout:ClientSecret"];
            var signatureClientId = _configuration["CashfreePayout:SignatureClientId"] ?? clientId;
           // var signatureClientId = _configuration["CashfreePayout:SignatureClientId"] ?? clientId;

            // default the beneficiary ID if the caller didn't supply one
            if (string.IsNullOrWhiteSpace(request.BeneficiaryId))
            {
                request.BeneficiaryId = $"BUS_{request.BusRegId}";
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

            using var httpRequest = new HttpRequestMessage(
                HttpMethod.Post,
                "https://sandbox.cashfree.com/payout/beneficiary");

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

            // --- save to DB, the missing piece ---
            var accountDetail = await _repo.GetAccountDetailByBusRegId(request.BusRegId);
            if (accountDetail != null)
            {
                accountDetail.CashfreeBeneficiaryId = cfResponse.BeneficiaryId;
                accountDetail.CashfreeBeneficiaryStatus = cfResponse.BeneficiaryStatus;
                accountDetail.CashfreeBeneficiaryCreatedDate = DateTime.UtcNow;

                await _repo.UpdateBusinessAccountDetails(accountDetail);
            }
            else
            {
                _logger.LogWarning(
                    "Beneficiary created on Cashfree but no BusinessAccountDetail found for BusRegId {BusRegId}",
                    request.BusRegId);
            }
            // --- end save ---

            return cfResponse;
        }
    }
}
