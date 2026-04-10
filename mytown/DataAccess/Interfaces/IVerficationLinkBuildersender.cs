namespace mytown.DataAccess.Interfaces
{
    public interface IVerficationLinkBuildersender
    {

        /// <summary>
        /// Constructs the verification link using the frontend base URL and the token.
        /// </summary>
        /// <param name="frontendBaseUrl">The base URL for the frontend.</param>
        /// <param name="token">The verification token.</param>
        /// <returns>The full verification link.</returns>
        string BuildLink(string frontendBaseUrl, string token);
    }
}
