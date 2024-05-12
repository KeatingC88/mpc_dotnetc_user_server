namespace dotnet_user_server.Models 
{
    public class Login_EmailAddressTbl
    {
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public string? Email { get; set; } = string.Empty;
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
    }
}
