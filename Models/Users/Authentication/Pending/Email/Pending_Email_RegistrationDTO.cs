using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email
{
    public class Pending_Email_RegistrationDTO
    {
        [Required]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public string Client_time { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

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
        public string Screen_extend { get; set; } = string.Empty;

        [Required]
        public string Color_depth { get; set; } = string.Empty;

        [Required]
        public string Pixel_depth { get; set; } = string.Empty;

        [Required]
        public string Window_width { get; set; } = string.Empty;

        [Required]
        public string Window_height { get; set; } = string.Empty;

        public ulong Client_Time_Parsed { get; set; }

        public string Client_Networking_IP_Address { get; set; } = string.Empty;

        public int Client_Networking_Port { get; set; }

        public string Server_Networking_IP_Address { get; set; } = string.Empty;

        public int Server_Networking_Port { get; set; }

        public string Client_user_agent { get; set; } = string.Empty;

        public string Server_user_agent { get; set; } = string.Empty;

    }
}
