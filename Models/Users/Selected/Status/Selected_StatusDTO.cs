using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Status
{
    public class Selected_StatusDTO
    {
        public byte Online { get; set; }
        public byte Offline { get; set; }
        public byte Hidden { get; set; }
        public byte Away { get; set; }
        public byte DND { get; set; }
        public byte Custom { get; set; }
        public byte Online_status { get; set; }
        public string Custom_lbl { get; set; } = string.Empty;
        public ulong User_id { get; set; }

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
