namespace mpc_dotnetc_user_server.Models.Users.Authentication.JWT
{
    public class JWT_DTO
    {
        public byte Account_type { get; set; }
        public ulong End_User_ID { get; set; }
        public string User_roles { get; set; } = string.Empty;
        public string User_groups { get; set; } = string.Empty;
        public string Email_address { get; set; } = string.Empty;
    }
}
