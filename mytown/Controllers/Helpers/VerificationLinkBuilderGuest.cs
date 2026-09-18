using System;
using mytown.DataAccess.Interfaces;

namespace mytown.Controllers.Helpers
{
    public class VerificationLinkBuilderGuest : IVerificationLinkBuilderGuest
    {
        public string BuildLink(string frontendBaseUrl, string token)
        {
            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
                throw new ArgumentException("Frontend base URL must not be null or empty.", nameof(frontendBaseUrl));

            return $"{frontendBaseUrl.TrimEnd('/')}/verify-guest-email?token={token}";
        }
    }
}