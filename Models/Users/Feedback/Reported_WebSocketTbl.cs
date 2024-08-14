namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_WebSocketTbl
    {
        public ulong ID { get; set; }
        public ulong USER_ID { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong User { get; set; }
        public byte Spam { get; set; } 
        public byte Abuse  { get; set; } 
        public string Reason { get; set; } = string.Empty;
    }
}
