namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Contact_UsDTO
    {
        public ulong ID { get; set; }
        public ulong USER_ID { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string Subject_line { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}