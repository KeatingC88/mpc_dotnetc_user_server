using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_AvatarDTO
    {
        public ulong User_id { get; set; }
        public string Avatar_title { get; set; } = string.Empty;
        public string Avatar_url_path { get; set; } = string.Empty;
        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
