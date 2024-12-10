using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication
{
    public class Login_PasswordDTO
    {
        public ulong User_id { get; set; }
        public byte[]? Password { get; set; }
        public byte[]? New_password { get; set; }
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}
