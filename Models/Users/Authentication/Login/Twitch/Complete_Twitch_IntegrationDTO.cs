using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch
{
    public class Complete_Twitch_IntegrationDTO
    {

        [Required(ErrorMessage = "Email Address is Missing.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Twitch ID is Missing.")]
        public ulong Twitch_ID { get; set; }

        [Required(ErrorMessage = "End User ID is Missing.")]
        public ulong End_User_ID { get; set; }

    }
}
