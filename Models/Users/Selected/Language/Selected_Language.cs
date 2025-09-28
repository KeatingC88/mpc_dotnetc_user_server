using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Language
{
    public class Selected_Language
    {
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public long Updated_on { get; set; }
        [Required]
        public long Updated_by { get; set; }
        [Required]
        public string Language { get; set; } = string.Empty;
        [Required]
        public string Region { get; set; } = string.Empty;
    }
}
