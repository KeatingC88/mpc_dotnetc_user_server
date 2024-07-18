namespace mpc_dotnetc_user_server.Models.Users.Chat
{
    public class Chat_WebSocketLogTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Sent_to { get; set; }
        public bool Deleted { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public byte Approved { get; set; }
        public byte Requested { get; set; }
        public byte Blocked { get; set; }
    }
}
