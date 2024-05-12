namespace dotnet_user_server.Models.Users.Feedback
{
    public class Comment_BoxTbl
    {
        public ulong ID { get; set; }
        public ulong USER_ID { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public string Comment { get; set; } = string.Empty;

    }

    public class Comment_BoxDTO
    {
        public ulong ID { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
    }
}
