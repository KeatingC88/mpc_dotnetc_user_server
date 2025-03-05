namespace mpc_dotnetc_user_server.Models.Users.WebSocket_Chat
{
    public class WebSocket_Chat_PermissionTbl
    {
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Participant_ID { get; set; }
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
