using Microsoft.Extensions.Options;
using MyTown.Configurations;
using MyTown.DTOs.Razorpay;
using MyTown.Services.Interfaces;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MyTown.Services.Implementations
{
    public class RazorpayService : IRazorpayService
    {
        private readonly RazorpayXSettings _settings;
        private readonly HttpClient _httpClient;

        public RazorpayService(
            IOptions<RazorpayXSettings> settings,
            HttpClient httpClient)
        {
            _settings = settings.Value;
            _httpClient = httpClient;
        }

        public async Task<CreateContactResponseDto> CreateContactAsync(
            CreateContactRequestDto request)
        {
            // Razorpay uses KeyId:KeySecret for Basic Authentication
            var credentials = Convert.ToBase64String(
                Encoding.UTF8.GetBytes(
                    $"{_settings.KeyId}:{_settings.KeySecret}"
                ));

            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", credentials);

            var json = JsonSerializer.Serialize(request);

            using var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");

            var url = $"{_settings.BaseUrl.TrimEnd('/')}/contacts";

            var response = await _httpClient.PostAsync(url, content);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception(
                    $"Razorpay Contact creation failed. " +
                    $"Status: {response.StatusCode}, " +
                    $"Response: {responseBody}");
            }

            var result =
                JsonSerializer.Deserialize<CreateContactResponseDto>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

            if (result == null)
            {
                throw new Exception(
                    "Unable to read Razorpay Contact response.");
            }

            return result;
        }
    }
}