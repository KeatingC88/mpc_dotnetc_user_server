using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_Broken_LinkDTO
    {
        [Required(ErrorMessage = "URL is missing.")]
        public string URL { get; set; } = string.Empty;

        [Required(ErrorMessage="Token is missing.")]
        public string Token { get; set; } = string.Empty;
    }
}
