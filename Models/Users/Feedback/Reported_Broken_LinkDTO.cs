using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_Broken_LinkDTO
    {
        [Required(ErrorMessage = "URL is missing.")]
        public string URL { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public ulong ID { get; set; }
    }
}
