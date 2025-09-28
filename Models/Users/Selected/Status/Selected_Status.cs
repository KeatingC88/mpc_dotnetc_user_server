using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Status
{
    public class Selected_Status
    {
        [Required]
        public long Status { get; set; }
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public string Custom_lbl { get; set; } = string.Empty;
    }
}
