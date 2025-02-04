using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email
{
    public class Login_PasswordDTO
    {
        public ulong User_id { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? New_password { get; set; }
        public string Email_Address { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
