using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Confirmation_Email_Registration_EncryptedDTO
    {
        public string Email_Address { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
