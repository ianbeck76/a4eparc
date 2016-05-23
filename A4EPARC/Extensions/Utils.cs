using System.Text.RegularExpressions;

namespace A4EPARC.Extensions
{
    public class Utils
    {
        public static bool IsValidEmail(string email)
        {
            string pattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                             + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                             + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        public static int ExtractNumber(string text)
        {
            Match match = Regex.Match(text, @"(\d+)");
            if (match == null)
            {
                return 0;
            }

            int value;
            if (!int.TryParse(match.Value, out value))
            {
                return 0;
            }

            return value;
        }
    }
}