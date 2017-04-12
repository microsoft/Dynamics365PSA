using System.Text.RegularExpressions;

namespace Common.Utilities.Authentication
{
    public class CrmUrlVerification
    {
        public static bool ValidateCrmUrl(string attemptedMatch)
        {
            const string pattern = @"^(https:\/\/)([\da-zA-Z-]+)\.crm([\d]{0,2})\.(dynamics|crmlivetie|dynamics-int)\.com$";
            Regex ex = new Regex(pattern);

            return ex.IsMatch(attemptedMatch);
        }
    }
}
