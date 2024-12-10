namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_WebSocket_AbuseTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public byte Type { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Abuser { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
