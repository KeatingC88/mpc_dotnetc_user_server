using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Pending_Email_RegistrationDTO
    {
        [Required(ErrorMessage = "Email Address is Missing.")]
        [EmailAddress(ErrorMessage = "Email Address format is 'local@domain.com'.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Region is Missing.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Language_Region length must equal 5.")]
        public string Language_Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Code must equal greater than 8.")]
        public string Code { get; set; } = string.Empty;
    }
}
