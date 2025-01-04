using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Reported
{
    public class Report_Email_RegistrationTbl
    {
        public ulong User_id { get; set; }
        public bool Deleted { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Created_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Client_time { get; set; }
        public string Client_Networking_IP_Address { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
    }
}