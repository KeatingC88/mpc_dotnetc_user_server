using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Report
{
    public class Report_Failed_Authorization_History
    {
        [Required]
        public long ID { get; set; }

        [Required]
        public long End_User_ID { get; set; }

        [Required]
        public long Client_id { get; set; }

        [Required]
        public long JWT_id { get; set; }

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Login_type { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public long Client_time { get; set; }
        [Required]
        public string Remote_IP { get; set; } = string.Empty;

        [Required]
        public int Remote_Port { get; set; }

        [Required]
        public string Server_IP { get; set; } = string.Empty;

        [Required]
        public int Server_Port { get; set; }

        [Required]
        public string Client_IP { get; set; } = string.Empty;

        [Required]
        public int Client_Port { get; set; }

        [Required]
        public string Controller { get; set; } = string.Empty;

        [Required]
        public string Action { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Server_User_Agent { get; set; } = string.Empty;

        [Required]
        public string Client_User_Agent { get; set; } = string.Empty;

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

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;
        public long Client_Time_Parsed { get; set; }

        public string User_agent { get; set; } = string.Empty;
        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;

        public string Reason { get; set; } = string.Empty;

        public bool Deleted { get; set; }

        public long Updated_by { get; set; }
        public long Created_on { get; set; }
        public long Created_by { get; set; }
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }

        public string Language_Region { get; set; } = string.Empty;
        public string Email_Address { get; set; } = string.Empty;

    }
}


