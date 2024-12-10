using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Identity
{
    public class IdentityDTO
    {
        [Required]
        public ulong User_id { get; set; }
        public byte Gender { get; set; }

        [StringLength(int.MaxValue, MinimumLength = 2, ErrorMessage = "First Name must equal greater than 2.")]
        public string? First_name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, MinimumLength = 1, ErrorMessage = "Middle Name must equal greater than 1.")]
        public string? Middle_name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, MinimumLength = 2, ErrorMessage = "Last Name must equal greater than 2.")]
        public string? Last_name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, MinimumLength = 2, ErrorMessage = "Maiden Name must equal greater than 2.")]
        public string? Maiden_name { get; set; } = string.Empty;

        [StringLength(int.MaxValue, MinimumLength = 5, ErrorMessage = "Ethnicity Length must equal greater than 5.")]
        public string? Ethnicity { get; set; } = string.Empty;

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}