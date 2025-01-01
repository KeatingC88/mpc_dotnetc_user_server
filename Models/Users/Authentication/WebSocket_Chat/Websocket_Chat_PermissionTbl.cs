namespace mpc_dotnetc_user_server.Models.Users.Authentication.WebSocket_Chat
{
    public class Websocket_Chat_PermissionTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }//Person Making Modifications like an admin
        public ulong User_A_id { get; set; }//Conversation Person A
        public ulong User_B_id { get; set; }//Conversation Person B
        public bool Deleted { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public byte Approved { get; set; }
        public byte Requested { get; set; }
        public byte Blocked { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
