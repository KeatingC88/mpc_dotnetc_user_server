namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Pending_Telephone_RegistrationTbl
    {
        public ulong ID { get; set; }
        public byte? Country { get; set; }
        public string? Phone { get; set; } = string.Empty;
        public string? Carrier { get; set; } = string.Empty;
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string? Code { get; set; } = string.Empty;
    }
}
