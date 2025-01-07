using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Account_Roles {
    public class Account_RolesDTO
    {
        public ulong User_id { get; set; }
        public string Roles { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
