namespace mpc_dotnetc_user_server.Models.Users.Authentication.Account_Groups
{
    public class Account_GroupsTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public string Groups { get; set; } = string.Empty;
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public bool Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
    }
}
