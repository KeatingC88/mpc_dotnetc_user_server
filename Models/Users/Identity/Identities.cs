using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Identity
{
    public class Identities
    {
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
        public byte Month { get; set; }
        public byte Day { get; set; }
        public long Year { get; set; }
        public string? First_name { get; set; } = string.Empty;
        public string? Middle_name { get; set; } = string.Empty;
        public string? Last_name { get; set; } = string.Empty;
        public string? Maiden_name { get; set; } = string.Empty;
        public string? Ethnicity { get; set; } = string.Empty;
    }
}