using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock
{
    public class Selected_Navbar_Lock
    {
        [Required]
        public bool Locked { get; set; }
        [Required]
        public long End_User_ID { get; set; }
    }
}
