using Microsoft.Extensions.Configuration;
using dotnet_user_server.Controllers.Users.Account;
using dotnet_user_server.Models;
using System.Text.RegularExpressions;

namespace dotnet_user_server.Controllers
{
    public class Valid
    {
        static public bool Email(string email)
        {
            char symbol = '@';
            bool hasAtSymbol = email.Contains(symbol);
            if (!hasAtSymbol)
                return false;

            symbol = '.';
            bool hasDot = email.Contains(symbol);
            if (!hasDot)
                return false;

            string[] emailHasTwoParts = email.Split('@');
            if (emailHasTwoParts[0].Length < 2)
                return false;

            if (emailHasTwoParts[1].Length < 7)
                return false;

            var trimmedEmail = email.Trim();
            if (trimmedEmail.EndsWith("."))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                if (addr.Address != trimmedEmail)
                    return false;
            }
            catch
            {
                return false;
            }

            return true;
        }
        static public bool Password(string password)
        {
            Regex check = new Regex(@"/^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*\W)[A-Za-z\d\W]{8,}$/gm");
            if (check.IsMatch(password))
                return false;

            return true;
        }
        static public bool Phone(string phone)
        {
            return true;
        }
        static public bool LanguageRegion(string language_region)
        {
            char symbol = '-';
            bool hasAtSymbol = language_region.Contains(symbol);
            if (!hasAtSymbol)
                return false;

            string[] language_region_has_two_parts_to_it = language_region.Split('-');
            if (!Regex.IsMatch(language_region_has_two_parts_to_it[0], @"^[a-zA-Z]+$"))
                return false;

            if (!Regex.IsMatch(language_region_has_two_parts_to_it[1], @"^[a-zA-Z]+$"))
                return false;

            if (!language_region_has_two_parts_to_it[0].All(letter => char.IsLower(letter)))
                return false;

            if (!language_region_has_two_parts_to_it[1].All(letter => char.IsUpper(letter)))
                return false;

            switch (language_region.ToUpper()) {
                case "CN-TW":
                case "CN-HK":
                case "CN-MO":
                case "CN-ZH":
                case "CN-CM":
                case "CN-YU":
                case "CN-NA":
                case "EN-US":
                case "ES-MX":
                case "ES-US":
                case "NL-BE":
                case "NL-NL":
                    return true;
            }
            return false;
        }
    }
}
