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

        public static int ExtractMaxDays(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return 0;

            // Examples:
            // "3-5 days"
            // "2–4"
            // "5 days"
            // "7"

            input = input.ToLower()
                         .Replace("days", "")
                         .Replace("day", "")
                         .Trim();

            // Normalize dash types
            input = input.Replace("–", "-");

            if (input.Contains("-"))
            {
                var parts = input.Split('-');
                if (int.TryParse(parts.Last().Trim(), out int max))
                    return max;
            }

            if (int.TryParse(input.Trim(), out int single))
                return single;

            return 0;
        }
    }

}
