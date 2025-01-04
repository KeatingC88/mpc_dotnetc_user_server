namespace mpc_dotnetc_user_server.Models.Users.Selected.Status
{
    public class Selected_StatusTbl
    {
        public bool Deleted { get; set; }
        public byte Online { get; set; }
        public byte Offline { get; set; }
        public byte Hidden { get; set; }
        public byte Away { get; set; }
        public byte DND { get; set; }
        public byte Custom { get; set; }
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public string Custom_lbl { get; set; } = string.Empty;
    }
}
