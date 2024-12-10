using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_ThemeDTO
    {
        public ulong User_id { get; set; }
        public byte? Theme { get; set; }
        public byte Light { get; set; }
        public byte Night { get; set; }
        public byte Custom { get; set; }

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
