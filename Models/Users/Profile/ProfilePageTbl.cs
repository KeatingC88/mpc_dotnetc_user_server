namespace mpc_dotnetc_user_server.Models.Users.Profile
{
    public class ProfilePageTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string? Page_Title { get; set; } = string.Empty;
        public string? Page_Description { get; set; } = string.Empty;
        public string? About_Me { get; set; } = string.Empty;
        public string? Banner_URL { get; set; } = string.Empty;

    }
}