using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps
{
    public class Login_Time_StampDTO
    {
        public long End_User_ID { get; set; }

        [Required(ErrorMessage = "Application Login Time is Missing.")]
        public ulong Login_on { get; set; }

        [Required(ErrorMessage = "Application Client Time is Missing.")]
        public string Client_time { get; set; } = string.Empty;

        public long Client_Time_Parsed { get; set; }

        [Required(ErrorMessage = "Application Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Server_IP { get; set; } = string.Empty;
        public string Client_IP { get; set; } = string.Empty;
        public int Client_Port { get; set; }
        public int Server_Port { get; set; }

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
