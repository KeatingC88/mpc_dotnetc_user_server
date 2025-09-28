using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Alignment
{
    public class Selected_App_Grid_Type
    {
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public byte Grid { get; set; }
    }
}