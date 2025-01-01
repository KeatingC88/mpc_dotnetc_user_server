using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email
{
    public class Pending_Email_Registration_EncryptedDTO
    {
        [Required]
        public string Email_Address { get; set; } = string.Empty;
        [Required]
        public string Language { get; set; } = string.Empty;
        [Required]
        public string Region { get; set; } = string.Empty;
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
