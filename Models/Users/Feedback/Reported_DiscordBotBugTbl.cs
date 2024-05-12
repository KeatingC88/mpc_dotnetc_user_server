namespace dotnet_user_server.Models.Users.Feedback
{
    public class Reported_DiscordBotBugTbl
    {
        public ulong ID { get; set; }
        public ulong USER_ID { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
    public class Discord_Bot_BugDTO
    {
        public ulong ID { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
    }
}
