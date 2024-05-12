namespace dotnet_user_server.Models.Users.Confirmation
{
    public class Unconfirmed_EmailAddressTbl
    {
        public ulong ID { get; set; }
        public string? Email { get; set; } = string.Empty;
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string? Language_Region { get; set; } = string.Empty;
        public string? Code { get; set; } = string.Empty;
    }
}
