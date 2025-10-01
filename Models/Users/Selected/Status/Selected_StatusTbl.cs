using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Status
{
    public class Selected_StatusTbl
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public bool Online { get; set; }
        [Required]
        public bool Offline { get; set; }
        [Required]
        public bool Hidden { get; set; }
        [Required]
        public bool Away { get; set; }
        [Required]
        public bool DND { get; set; }
        [Required]
        public bool Custom { get; set; }
        [Required]
        public string Custom_lbl { get; set; } = string.Empty;
        [Required]
        public long Updated_on { get; set; }
        [Required]
        public long Updated_by { get; set; }
        public bool Deleted { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
    }
}
