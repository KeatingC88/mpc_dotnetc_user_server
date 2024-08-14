namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_ProfileTbl
    {
        public ulong ID { get; set; }
        public ulong USER_ID { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Reported_ID { get; set; }
        public string? Avatar_Title { get; set; } = string.Empty;
        public string? Avatar_URL { get; set; } = string.Empty;
        public string? Page_Title { get; set; } = string.Empty;
        public string? Page_Description { get; set; } = string.Empty;
        public string? About_Me { get; set; } = string.Empty;
        public string? Banner_URL { get; set; } = string.Empty;
        public string Reported_Reason { get; set; } = string.Empty;
        public ulong? Report_Chat_TS { get; set; }
    }
}
