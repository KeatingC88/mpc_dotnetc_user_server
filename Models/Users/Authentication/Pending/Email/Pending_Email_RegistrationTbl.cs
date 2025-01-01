using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email
{
    public class Pending_Email_RegistrationTbl
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
        public string Code { get; set; } = string.Empty;
    }
}