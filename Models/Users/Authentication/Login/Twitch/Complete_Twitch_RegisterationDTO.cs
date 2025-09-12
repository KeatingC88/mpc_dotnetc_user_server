using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch
{
    public class Complete_Twitch_RegisterationDTO
    {
        [Required(ErrorMessage = "Name is Missing.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is Missing.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Missing.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Code is Missing.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Twitch Name is Missing.")]
        public string Twitch_Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public ulong Client_time { get; set; }

        public ulong Client_Time_Parsed { get; set; }

        [Required(ErrorMessage ="Twitch ID is Missing.")]
        public ulong Twitch_ID { get; set; }

        [Required(ErrorMessage = "Theme is Missing.")]
        public byte Theme { get; set; }

        [Required(ErrorMessage = "Alignment is Missing.")]
        public byte Alignment { get; set; }

        [Required(ErrorMessage = "Text_alignment is Missing.")]
        public byte Text_alignment { get; set; } 

        [Required(ErrorMessage = "Nav_Lock is Missing.")]
        public bool Nav_lock { get; set; } 

        [Required(ErrorMessage = "Grid Type is Missing.")]
        public byte Grid_type { get; set; }

        [Required(ErrorMessage = "JWT Issuer Key is Missing.")]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Client Key is Missing.")]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Client Address is Missing.")]
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

        [Required]
        public string Remote_IP { get; set; } = string.Empty;

        [Required]
        public int Remote_Port { get; set; }

        [Required]
        public string Server_IP_Address { get; set; } = string.Empty;

        [Required]
        public int Server_Port { get; set; }

        [Required]
        public string Client_IP { get; set; } = string.Empty;

        [Required]
        public int Client_Port { get; set; }

        public ulong End_User_ID { get; set; }

    }
}
