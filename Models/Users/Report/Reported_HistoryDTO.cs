using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Report
{
    public class Reported_HistoryDTO
    {
        public long End_User_ID { get; set; }
        public string User { get; set; } = string.Empty;
        public byte Approved { get; set; }
        public byte Requested { get; set; }
        public byte Blocked { get; set; }
        [Required]
        public string Report_type { get; set; } = string.Empty;
        public ulong Block { get; set; }
        public ulong Spam { get; set; }
        public ulong Abuse { get; set; }
        public ulong Fake { get; set; }
        public ulong Nudity { get; set; }
        public ulong Violence { get; set; }
        public ulong Threat { get; set; }
        public ulong Misinform { get; set; }
        public ulong Harass { get; set; }
        public ulong Illegal { get; set; }
        public ulong Self_harm { get; set; }
        public ulong Disruption { get; set; }
        public ulong Hate { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Region { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Client_time { get; set; } = string.Empty;          
        public long Client_Time_Parsed { get; set; } 
        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Server_IP { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public int Server_Port { get; set; }
        public string Login_type { get; set; } = string.Empty;

        public long Client_id { get; set; }

        public long JWT_id { get; set; }

        public string JWT_issuer_key { get; set; } = string.Empty;

        public string JWT_client_key { get; set; } = string.Empty;

        public string JWT_client_address { get; set; } = string.Empty;

        public string User_agent { get; set; } = string.Empty;

        public string Down_link { get; set; } = string.Empty;

        public string Connection_type { get; set; } = string.Empty;

        public string RTT { get; set; } = string.Empty;

        public string Data_saver { get; set; } = string.Empty;

        public string Device_ram_gb { get; set; } = string.Empty;

        public string Orientation { get; set; } = string.Empty;

        public string Screen_width { get; set; } = string.Empty;

        public string Screen_height { get; set; } = string.Empty;

        public string Color_depth { get; set; } = string.Empty;

        public string Pixel_depth { get; set; } = string.Empty;

        public string Window_width { get; set; } = string.Empty;

        public string Window_height { get; set; } = string.Empty;
        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;
    }
}
