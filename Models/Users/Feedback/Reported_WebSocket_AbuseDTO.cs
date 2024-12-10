using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_WebSocket_AbuseDTO
    {
        [Required(ErrorMessage = "Application User ID is Missing.")]
        public ulong User_id { get; set; }

        [Required(ErrorMessage = "Abuse Type is Missing.")]
        public byte Abuse_type { get; set; }

        [Required(ErrorMessage = "Abuser ID is Missing.")]
        public ulong Abuser { get; set; }       

        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Reason must equal greater than 3.")]
        public string Reason { get; set; } = string.Empty;

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
