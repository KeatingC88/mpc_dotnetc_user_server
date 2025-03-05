using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Account_Groups
{
    public class Account_GroupsTbl
    {
        [Required]
        public ulong ID { get; set; }
        [Required]
        public ulong User_ID { get; set; }
        [Required]
        public string Groups { get; set; } = string.Empty;
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public bool Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        [Required]
        public ulong Updated_on { get; set; }
        [Required]
        public ulong Updated_by { get; set; }
    }
}
