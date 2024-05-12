namespace dotnet_user_server.Models.Users.Chat
{
    public class Chat_WebSocketDirectMessagesTbl
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
        public ulong HostClient_ts { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class Chat_WebSocketDirectMessagesDTO
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
        public ulong timestamp { get; set; }
        public string Token { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}
