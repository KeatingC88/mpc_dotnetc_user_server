﻿namespace mpc_dotnetc_user_server.Controllers.Users.JWT
{
    public class JWT_DTO
    {
        public byte Account_type { get; set; }
        public ulong User_id { get; set; }
        public string User_roles { get; set; } = string.Empty;
        public string User_groups { get; set; } = string.Empty;
        public string Email_address { get; set; } = string.Empty;
    }
}
