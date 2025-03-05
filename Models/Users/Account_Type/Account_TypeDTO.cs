using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Account_Type
{
    public class Account_TypeDTO
    {
        [Required]
        public ulong End_User_ID { get; set; }
        [Required]
        public byte Type { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
