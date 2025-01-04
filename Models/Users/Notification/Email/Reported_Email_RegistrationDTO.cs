using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Notification.Email
{
    public class Reported_Email_RegistrationDTO
    {
        [Required(ErrorMessage = "Email Address is Missing.")]
        [EmailAddress(ErrorMessage = "Email Address format is 'local@domain.com'.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Networking IP Address is Missing.")]
        public string Client_Networking_IP_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Networking Port is Missing.")]
        public int Client_Networking_Port { get; set; }

        [Required(ErrorMessage = "Server Network IP Address is Missing.")]
        public string Server_Networking_IP_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Server Networking Port is Missing.")]
        public int Server_Networking_Port { get; set; }

        [Required(ErrorMessage = "Language_Code is Missing.")]
        [StringLength(6, MinimumLength = 4, ErrorMessage = "Language_Region Code length must equal 2-3 letters.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        [StringLength(3, MinimumLength = 2, ErrorMessage = "Region Code length must equal 2-3 letters.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "User_id is Missing.")]
        public ulong User_ID { get; set; }

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public string Client_time { get; set; } = string.Empty;
    }
}
