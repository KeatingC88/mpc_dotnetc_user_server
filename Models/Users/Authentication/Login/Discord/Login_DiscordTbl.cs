namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord
{
    public class Discord_Email_AddressTbl
    {
        public long ID { get; set; }
        public long End_User_ID { get; set; }
        public string Email_Address { get; set; } = string.Empty;
        public long Created_on { get; set; }
        public long Created_by { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Updated_on { get; set; }
        public long Updated_by { get; set; }
    }
}
