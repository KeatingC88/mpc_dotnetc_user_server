namespace mpc_dotnetc_user_server.Models.Users.Authentication
{
    public class Logout_Time_StampTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Logout_on { get; set; }
        public bool Deleted { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Created_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
    }
}
