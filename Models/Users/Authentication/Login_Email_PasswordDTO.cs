using System.ComponentModel.DataAnnotations;
using mpc_dotnetc_user_server.Models.Users.Selection;

namespace mpc_dotnetc_user_server.Models.Users.Authentication
{
    public class Login_Email_PasswordDTO
    {
        [Required]
        [StringLength(25, ErrorMessage = "{0} length must be between {2} and {1]", MinimumLength = 9)]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        [StringLength(25, ErrorMessage = "{0} length must be between {2} and {1]", MinimumLength = 8)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Language Code length must equal 2-3 letters.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Region Code length must equal 2-3 letters.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alignment values must be 0, 1, xor 2.")]
        public AlignmentType Alignment { get; set; }

        [Required(ErrorMessage = "Alignment values must be 0, 1, xor 2.")]
        public AlignmentType Text_alignment { get; set; }

        [Required]
        public bool Locked { get; set; }

        [Required(ErrorMessage = "Application Theme is Missing.")]
        public byte Theme { get; set; }
    }
}