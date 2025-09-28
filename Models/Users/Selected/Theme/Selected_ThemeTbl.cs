using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_ThemeTbl
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public bool Light { get; set; }
        [Required]
        public bool Night { get; set; }
        [Required]
        public bool Custom { get; set; }
        public bool Deleted { get; set; }
        public long Updated_by { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
    }
}
