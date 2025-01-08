namespace mpc_dotnetc_user_server.Controllers.Users.JWT
{
    public class JWT_DTO
    {
        public byte account_type { get; set; }
        public ulong user_id { get; set; }
        public string user_roles { get; set; } = string.Empty;
        public string user_groups { get; set; } = string.Empty;
    }
}
