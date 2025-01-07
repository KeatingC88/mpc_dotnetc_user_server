using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Account_Groups {
    public class Account_GroupsDTO
    {
        public ulong User_id { get; set; }
        public string Groups { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
