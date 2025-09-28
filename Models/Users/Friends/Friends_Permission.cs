using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mpc_dotnetc_user_server.Models.Users.Friends
{
    public class Friends_Permission
    {

        [Required]
        public long End_User_ID { get; set; }

        [Required]
        public long Participant_ID { get; set; }

        [Required]
        public bool Approved { get; set; }

        [Required]
        public bool Requested { get; set; }

        [Required]
        public bool Blocked { get; set; }

        [Required]
        public long Updated_by { get; set; }

        public long Created_on { get; set; }
        [Required]
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public bool Deleted { get; set; }
        public long Created_by { get; set; }
    }
}