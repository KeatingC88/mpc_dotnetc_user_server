using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email
{
    public class Login_PasswordTbl
    {
        [Required]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long ID { get; set; }

        [Required]
        public long End_User_ID { get; set; }

        [Required]
        public byte[]? Password { get; set; }

        [Required]
        public long Updated_on { get; set; }

        [Required]
        public long Updated_by { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
    }
}
