
using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_ThemeDTO
    {
        public ulong User_id { get; set; }
        [Required(ErrorMessage = "Application Theme is Missing.")]
        public byte Theme { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
