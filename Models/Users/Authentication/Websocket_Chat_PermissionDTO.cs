using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication
{
    public class Websocket_Chat_PermissionDTO
    {
        public ulong User_id { get; set; }
        public ulong User_A_id { get; set; }
        public ulong User_B_id { get; set; }
        public byte Approved { get; set; }
        public byte Requested { get; set; }
        public byte Blocked { get; set; }
        public string Message { get; set; } = string.Empty;

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
