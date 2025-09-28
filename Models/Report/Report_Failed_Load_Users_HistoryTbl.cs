using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Report
{
    public class Report_Failed_User_Agent_HistoryTbl
    {
        public long ID { get; set; }
        public long End_User_ID { get; set; }
        public bool Deleted { get; set; }
        public long Updated_by { get; set; }
        public long Created_on { get; set; }
        public long Created_by { get; set; }
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Client_time { get; set; }
        public string Client_IP { get; set; } = string.Empty;
        public string Server_IP { get; set; } = string.Empty;
        public int Client_Port { get; set; }
        public int Server_Port { get; set; }
        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Language_Region { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Login_type { get; set; } = string.Empty;
        [Required]
        public string User_agent { get; set; } = string.Empty;
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

    }
}


