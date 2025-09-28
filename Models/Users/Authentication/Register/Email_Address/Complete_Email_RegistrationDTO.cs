using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address
{
    public class Complete_Email_RegistrationDTO
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public byte Alignment { get; set; }

        [Required]
        public byte Text_alignment { get; set; }

        [Required]
        public bool Nav_lock { get; set; }

        [Required]
        public byte Theme { get; set; }

        [Required]
        public byte Grid_type { get; set; }

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

        public long End_User_ID { get; set; }
        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;
    }
}
