using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Account_Groups
{
    public class Account_Group
    {
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public string Groups { get; set; } = string.Empty;
    }
}
