using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.WebSocket_Chat
{
    public class WebSocket_Chat_Permission
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Login_type { get; set; } = string.Empty;

        [Required]
        public long Client_id { get; set; }

        [Required]
        public long JWT_id { get; set; }

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

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

        public string ID { get; set; } = string.Empty;
        public long End_User_ID { get; set; }
        public long Deleted_by { get; set; }
        public string User { get; set; } = string.Empty;
        public long Participant_ID { get; set; }
        public byte Approved { get; set; }
        public byte Requested { get; set; }
        public byte Blocked { get; set; }
        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;
        public string Client_time { get; set; } = string.Empty;
        public long Client_Time_Parsed { get; set; }
        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Server_IP { get; set; } = string.Empty;
        public int Server_Port { get; set; }
        public string Reason { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;

    }
}
