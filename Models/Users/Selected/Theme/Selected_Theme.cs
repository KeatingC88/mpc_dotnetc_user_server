using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_Theme
    {
        [Required]
        public byte Theme { get; set; }
        [Required]
        public long End_User_ID { get; set; }
    }
}
