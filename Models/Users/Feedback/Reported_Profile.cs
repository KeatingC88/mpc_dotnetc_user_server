using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_Profile
    {
        [Required]
        public long End_User_ID { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        [Required]
        public long Updated_on { get; set; }
        [Required]
        public long Updated_by { get; set; }
        [Required]
        public long Reported_ID { get; set; }
        public string? Avatar_Title { get; set; } = string.Empty;
        public string? Avatar_URL { get; set; } = string.Empty;
        public string? Page_Title { get; set; } = string.Empty;
        public string? Page_Description { get; set; } = string.Empty;
        public string? About_Me { get; set; } = string.Empty;
        public string? Banner_URL { get; set; } = string.Empty;
        public string Reported_reason { get; set; } = string.Empty;
        public long? Report_Chat_TS { get; set; }
    }
}
