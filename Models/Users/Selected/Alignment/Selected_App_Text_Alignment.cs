using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Alignment
{
    public class Selected_App_Text_Alignment
    {
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public byte Text_alignment { get; set; }
    }
}
