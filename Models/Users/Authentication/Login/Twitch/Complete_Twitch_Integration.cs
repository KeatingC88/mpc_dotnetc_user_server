using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch
{
    public class Complete_Twitch_Integration
    {
        [Required]
        public string Twitch_User_Name { get; set; } = string.Empty;

        [Required]
        public string Email_Address { get; set; } = string.Empty;

        [Required]
        public string Code { get; set; } = string.Empty;

        [Required]
        public long Twitch_ID { get; set; }

        [Required]
        public long End_User_ID { get; set; }

    }
}
