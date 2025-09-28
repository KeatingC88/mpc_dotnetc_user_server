using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Name
{
    public class Selected_Name
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public long Updated_by { get; set; }
        [Required]
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }

    }
}
