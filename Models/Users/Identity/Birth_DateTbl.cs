using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Identity
{
    public class Birth_DateTbl
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
        public byte? Month { get; set; }
        [Required]
        public byte? Day { get; set; }
        [Required]
        public long? Year { get; set; }
        public long? Created_by { get; set; }
        public long? Created_on { get; set; }
        public bool? Deleted { get; set; }
        public long? Deleted_on { get; set; }
        public long? Deleted_by { get; set; }
    }
}