using System;
using mytown.DataAccess.Interfaces;

namespace mytown.Controllers.Helpers
{
    public class VerificationLinkBuilderbusiness : IVerificationLinkBuilderbusiness
    {
        public string BuildLink(string frontendBaseUrl, string token)
        {
            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new ArgumentException("Frontend base URL must not be null or empty.", nameof(frontendBaseUrl));
            }

            return $"{frontendBaseUrl.TrimEnd('/')}/?verify-business-email&token={token}";
        }
    }
}
