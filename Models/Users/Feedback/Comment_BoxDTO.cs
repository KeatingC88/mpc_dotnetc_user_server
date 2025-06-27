using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Comment_BoxDTO
    {
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }

        public string Comment { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string ID { get; set; } = string.Empty;
        public ulong End_User_ID { get; set; }
        public string Detail { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language is Missing.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is Missing.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public string Client_time { get; set; } = string.Empty;

        public ulong Client_Time_Parsed { get; set; }
        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Server_IP_Address { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public int Server_Port { get; set; }

        [Required]
        public string Login_type { get; set; } = string.Empty;

        [Required]
        public ulong Client_id { get; set; }

        [Required]
        public ulong JWT_id { get; set; }

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

        [Required]
        public string User_agent { get; set; } = string.Empty;

        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;

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
