using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Language
{
    public class Selected_LanguageTbl
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long? ID { get; set; }
        [Required]
        public long? End_User_ID { get; set; }
        [Required]
        public long? Updated_on { get; set; }
        [Required]
        public long? Updated_by { get; set; }
        [Required]
        public string? Language_code { get; set; } = string.Empty;
        [Required]
        public string? Region_code { get; set; } = string.Empty;
        public long? Created_by { get; set; }
        public long? Created_on { get; set; }
    }
}
