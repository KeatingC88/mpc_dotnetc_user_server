using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Report
{
    public class Report_Failed_Email_Login_HistoryTbl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public string User_agent { get; set; } = string.Empty;

        [Required]
        public long Updated_by { get; set; }

        [Required]
        public string Down_link { get; set; } = string.Empty;

        [Required]
        public string Connection_type { get; set; } = string.Empty;

        [Required]
        public string RTT { get; set; } = string.Empty;

        [Required]
        public string Data_saver { get; set; } = string.Empty;

        [Required]
        public string Device_ram_gb { get; set; } = string.Empty;

        [Required]
        public string Orientation { get; set; } = string.Empty;

        [Required]
        public string Screen_width { get; set; } = string.Empty;

        [Required]
        public string Screen_height { get; set; } = string.Empty;

        [Required]
        public string Color_depth { get; set; } = string.Empty;

        [Required]
        public string Pixel_depth { get; set; } = string.Empty;

        [Required]
        public string Window_width { get; set; } = string.Empty;

        [Required]
        public string Window_height { get; set; } = string.Empty;
        public long? End_User_ID { get; set; }
        public bool Deleted { get; set; }
        public long Created_on { get; set; }
        public long Created_by { get; set; }
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Client_time { get; set; }
        public string Client_IP { get; set; } = string.Empty;
        public string Server_IP { get; set; } = string.Empty;
        public string Remote_IP { get; set; } = string.Empty;
        public int Client_Port { get; set; }
        public int Server_Port { get; set; }
        public int Remote_Port { get; set; }
        public string Language_Region { get; set; } = string.Empty;
        public string Email_Address { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
    }
}


