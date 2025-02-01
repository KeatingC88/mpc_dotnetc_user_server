using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Report
{
    public class Report_Failed_User_ID_HistoryDTO
    {
        [Required(ErrorMessage ="User ID is Missing.")]
        public ulong User_id { get; set; }  

        [Required(ErrorMessage = "Language is Missing.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is Missing.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public ulong Client_time { get; set; }
        public string Client_Networking_IP_Address { get; set; } = string.Empty;
        public int Client_Networking_Port { get; set; }
        public string Server_Networking_IP_Address { get; set; }  = string.Empty;
        public string Reason { get; set; }  = string.Empty;
        public int Server_Networking_Port { get; set; }
    }
}


