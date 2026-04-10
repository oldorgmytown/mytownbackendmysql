using mytown.DataAccess.Interfaces;

namespace mytown.Controllers.Helpers
{
    public class VerificationLinkBuildersender : IVerficationLinkBuildersender
    {
        public string BuildLink(string frontendBaseUrl, string token)
        {
            if (string.IsNullOrWhiteSpace(frontendBaseUrl))
            {
                throw new ArgumentException("Frontend base URL must not be null or empty.", nameof(frontendBaseUrl));
            }

            return $"{frontendBaseUrl.TrimEnd('/')}/?verify-sender-email&token={token}";
        }
    }
}
