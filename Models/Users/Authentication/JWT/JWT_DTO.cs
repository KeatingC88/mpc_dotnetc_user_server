using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.JWT
{
    public class JWT_DTO
    {
        [Required]
        public byte Account_type { get; set; }

        [Required]
        public long End_User_ID { get; set; }

        [Required]
        public string User_roles { get; set; } = string.Empty;

        [Required]
        public string User_groups { get; set; } = string.Empty;

        [Required]
        public string Email_address { get; set; } = string.Empty;
    }
}
