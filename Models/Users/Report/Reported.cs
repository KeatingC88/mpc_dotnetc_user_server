using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Report
{
    public class Reported
    {
        [Required]
        public long End_User_ID { get; set; }

        [Required]
        public long Participant_ID { get; set; }

        [Required]
        public string Report_type { get; set; } = string.Empty;
    }
}
