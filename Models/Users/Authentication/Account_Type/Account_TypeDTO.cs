using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Account_Type {
    public class Account_TypeDTO
    {
        public ulong User_id { get; set; }
        public byte Type { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
