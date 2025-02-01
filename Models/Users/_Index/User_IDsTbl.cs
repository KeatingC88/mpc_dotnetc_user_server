namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class User_IDsTbl
    {
        public ulong ID { get; set; }
        public string Public_id { get; set; } = string.Empty;
        public string Secret_id { get; set; } = string.Empty;
        public ulong Created_on { get; set; }
        public ulong Created_by { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
    }
}
