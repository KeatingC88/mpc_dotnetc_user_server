using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email
{
    public class Pending_Email_Registration_HistoryTbl
    {
        public ulong ID { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string Email_Address { get; set; } = string.Empty;
        public string Language_Region { get; set; } = string.Empty;
        public string Client_IP { get; set; } = string.Empty;
        public int Client_Port { get; set; }
        public string Server_IP { get; set; } = string.Empty;
        public int Server_Port { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public ulong Client_time { get; set; }
    }
}