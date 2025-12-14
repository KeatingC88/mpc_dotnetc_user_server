using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Interfaces;
using System.Text.RegularExpressions;

namespace mpc_dotnetc_user_server.Services.Security
{
    public class Valid : IValid
    {
        public Valid()
        {

        }

        public bool Email(string email_address)
        {
            if (email_address.IsNullOrEmpty())
                return false;

            char at_symbol = '@';
            char dot_symbol = '.';

            bool hasAtSymbol = email_address.Contains(at_symbol);
            if (!hasAtSymbol)
                return false;

            bool hasDot = email_address.Contains(dot_symbol);
            if (!hasDot)
                return false;

            string[] email_address_has_two_parts = email_address.Split(at_symbol);

            if (email_address_has_two_parts[0].Length < 2)
                return false;

            if (email_address_has_two_parts[0].Contains(at_symbol))
                return false;

            if (email_address_has_two_parts[0].StartsWith(" "))
                return false;

            if (email_address_has_two_parts[0].EndsWith(dot_symbol))
                return false;

            if (email_address_has_two_parts[1].Length < 7)
                return false;

            if (email_address_has_two_parts[1].Contains(at_symbol))
                return false;

            var trimmedEmail = email_address.Trim();

            if (trimmedEmail.EndsWith(dot_symbol))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email_address);
                if (addr.Address != trimmedEmail)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
        public bool Password(string password)
        {
            Regex check = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W).{8,}$");

            if (!check.IsMatch(password))
                return false;

            return true;
        }
        public bool Language_Code(string language_code)
        {
            switch (language_code.ToUpper())
            {
                case "DE":
                case "EN":
                case "ZH":
                case "HI":
                case "FR":
                case "ES":
                case "NL":
                    return true;
            }

            return false;
        }
        public bool Region_Code(string region_code)
        {
            if (!Regex.IsMatch(region_code, @"^[A-Z]+$"))
                return false;

            switch (region_code.ToUpper())
            {
                case "DE":
                case "GF":
                case "MX":
                case "FR":
                case "US":
                case "ES":
                case "NL":
                case "IN":
                case "BE":
                case "RU":
                case "HK":
                case "MC":
                case "TW":
                case "CDO":
                case "CJY":
                case "CMN":
                case "CNP":
                case "CPX":
                case "CSH":
                case "CZH":
                case "CZO":
                case "GAN":
                case "HAK":
                case "HSN":
                case "LZH":
                case "MNP":
                case "NAN":
                case "WUU":
                case "YUE":
                    return true;
            }

            return false;
        }
    }
}