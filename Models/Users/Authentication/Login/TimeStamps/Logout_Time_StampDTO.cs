using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps
{
    public class Logout_Time_StampDTO
    {
        public ulong User_id { get; set; }

        [Required(ErrorMessage = "Application Logout Time is Missing.")]
        public ulong Logout_on { get; set; }

        [Required(ErrorMessage = "Application Client Time is Missing.")]
        public ulong Client_time { get; set; }

        [Required(ErrorMessage = "Application Logout Location is Missing.")]
        public string Location { get; set; } = string.Empty;
        public string Client_Networking_IP_Address { get; set; } = string.Empty;
        public int Client_Networking_Port { get; set; }
        public string Server_Networking_IP_Address { get; set; } = string.Empty;
        public int Server_Networking_Port { get; set; }
    }
}
