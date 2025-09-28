using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Alignment
{
    public class Selected_App_Alignment
    {
        [Required]
        public long End_User_ID { get; set; }

        [Required]
        public byte Alignment { get; set; }
    }
}
