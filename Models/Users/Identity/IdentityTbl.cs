using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Identity
{
    public class IdentityTbl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }
        [Required]
        public long End_User_ID { get; set; }
        [Required]
        public long Updated_on { get; set; }
        [Required]
        public long Updated_by { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public byte Gender { get; set; }
        public string? First_Name { get; set; } = string.Empty;
        public string? Middle_Name { get; set; } = string.Empty;
        public string? Last_Name { get; set; } = string.Empty;
        public string? Maiden_Name { get; set; } = string.Empty;
        public string? Ethnicity { get; set; } = string.Empty;
    }
}