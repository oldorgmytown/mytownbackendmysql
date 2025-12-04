using System.Text.RegularExpressions;

namespace mytown.Controllers.Helpers
{
    public static class CsvValidationHelper
    {
        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                var _ = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch { return false; }
        }

        public static bool IsValidPhone(string phone)
        {
            // Allows: 10 digits, +country formats, etc.
            return Regex.IsMatch(phone, @"^[0-9+\-\s]{7,15}$");
        }

        public static bool IsCommaSeparatedList(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return false;

            // "Hyderabad,Bengaluru,Mumbai"
            return value.Split(',')
                        .All(v => !string.IsNullOrWhiteSpace(v.Trim()));
        }
    }

}
