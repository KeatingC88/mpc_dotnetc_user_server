using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Account_Roles
{
    public class Account_Role
    {
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public string Roles { get; set; } = string.Empty;
    }
}
