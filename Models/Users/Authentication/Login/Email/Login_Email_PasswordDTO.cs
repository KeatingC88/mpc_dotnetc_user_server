using System.ComponentModel.DataAnnotations;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email
{
    public class Login_Email_PasswordDTO
    {
        [Required]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Alignment { get; set; } = string.Empty;

        [Required]
        public string Text_alignment { get; set; } = string.Empty;

        [Required]
        public string Locked { get; set; } = string.Empty;

        [Required]
        public string Theme { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Client_time { get; set; } = string.Empty;

        [Required]
        public string JWT_issuer_key { get;set; } = string.Empty;

        [Required]
        public string JWT_client_key { get;set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;
    }
}