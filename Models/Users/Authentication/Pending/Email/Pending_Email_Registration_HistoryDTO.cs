using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email
{
    public class Pending_Email_Registration_HistoryDTO
    {
        [Required(ErrorMessage = "Email Address is Missing.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Code is Missing.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public string Client_time { get; set; } = string.Empty;
        public ulong Client_Time_Parsed { get; set; }

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;
        [Required(ErrorMessage = "Client Networking IP Address is Missing.")]
        public string Client_Networking_IP_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Networking Port is Missing.")]
        public int Client_Networking_Port { get; set; }

        [Required(ErrorMessage = "Server Network IP Address is Missing.")]
        public string Server_Networking_IP_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Server Networking Port is Missing.")]
        public int Server_Networking_Port { get; set; }

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

        [Required]
        public string User_agent { get; set; } = string.Empty;
        [Required]

        public string Client_user_agent { get; set; } = string.Empty;
        [Required]

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

        public string Screen_extend { get; set; } = string.Empty;
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
