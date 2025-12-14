using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord
{
    public class Complete_Discord_Integration
    {
        [Required]
        public string Discord_User_Name { get; set; } = string.Empty;

        [Required]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public long Discord_ID { get; set; }

        [Required]
        public long End_User_ID { get; set; }

    }
}
