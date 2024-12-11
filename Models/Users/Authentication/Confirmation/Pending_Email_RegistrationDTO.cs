using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Pending_Email_RegistrationDTO
    {
        [Required(ErrorMessage = "Email Address is Missing.")]
        [EmailAddress(ErrorMessage = "Email Address format is 'local@domain.com'.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Language Code length must equal 2-3 letters.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Region Code length must equal 2-3 letters.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Code must equal greater than 8.")]
        public string Code { get; set; } = string.Empty;
    }
}
