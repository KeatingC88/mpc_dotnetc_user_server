using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_LanguageDTO
    {
        public ulong User_id { get; set; }
        public string Language { get; set; } = string.Empty;
        public string Language_code { get; set; } = string.Empty;
        public string Region_code { get; set; } = string.Empty;
        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
