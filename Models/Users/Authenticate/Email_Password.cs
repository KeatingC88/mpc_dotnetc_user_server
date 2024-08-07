using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authenticate
{
    public class Email_Password
    {
        [Required]
        [StringLength(25, ErrorMessage ="{0} length must be between {2} and {1]", MinimumLength = 9)]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        [StringLength(25, ErrorMessage = "{0} length must be between {2} and {1]", MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;
    }
}