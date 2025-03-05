using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Account_Roles
{
    public class Account_RolesDTO
    {
        [Required]
        public ulong End_User_ID { get; set; }
        [Required]
        public string Roles { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
