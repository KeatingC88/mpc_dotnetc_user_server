using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Complete_Email_RegistrationDTO
    {
        [Required(ErrorMessage = "Email Address is Missing.")]
        [EmailAddress(ErrorMessage = "Email Address format is 'local@domain.com'.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Region is Missing.")]
        [StringLength(5, MinimumLength = 5, ErrorMessage = "Language_Region length must equal 5.")]
        public string Language_Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Password must equal greater than 8.")]
        public string Password { get; set; } = string.Empty;
        public ulong? Created_by { get; set; }
        public byte? Alignment { get; set; }
        public byte? Text_alignment { get; set; }
        public bool? Nav_lock { get; set; }
        public byte? Theme { get; set; }
    }
}