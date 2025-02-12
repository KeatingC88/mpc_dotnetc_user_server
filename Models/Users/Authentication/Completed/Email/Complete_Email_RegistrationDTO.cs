using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email
{
    public class Complete_Email_RegistrationDTO
    {
        [Required(ErrorMessage = "Name is Missing.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email Address is Missing.")]
        [EmailAddress(ErrorMessage = "Email Address format is 'local@domain.com'.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Language Code length must be 2-3 letters.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Region Code length must be 2-3 letters.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Password must be greater than 8.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 16, ErrorMessage = "Code length must be greater than 16")]
        public string Code { get; set; } = string.Empty;
        public byte Alignment { get; set; }
        public byte Text_alignment { get; set; }
        public bool Nav_lock { get; set; }
        public byte Theme { get; set; }
        public byte Grid_type { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Client_time { get; set; } = string.Empty;
        public string Client_Networking_IP_Address { get; set; } = string.Empty;
        public int Client_Networking_Port { get; set; }
        public string Server_Networking_IP_Address { get; set; } = string.Empty;
        public int Server_Networking_Port { get; set; }

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