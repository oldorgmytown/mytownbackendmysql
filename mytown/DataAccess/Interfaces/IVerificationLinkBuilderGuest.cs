namespace mytown.DataAccess.Interfaces
{
    public interface IVerificationLinkBuilderGuest
    {
        string BuildLink(string frontendBaseUrl, string token);
    }
}