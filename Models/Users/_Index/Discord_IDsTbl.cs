using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class Discord_IDsTbl
    {
        [Required]
        [Range(1, ulong.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public ulong ID { get; set; }

        [Required]
        [Range(1, ulong.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public ulong User_ID { get; set; }

        [Required]
        [Range(1, ulong.MaxValue, ErrorMessage = "ID must be greater than 0.")]
        public ulong Discord_ID { get; set; }

        [Required]
        [Range(1, ulong.MaxValue, ErrorMessage = "End User ID must be greater than 0.")]
        public ulong Created_by { get; set; }

        [Range(0, 1, ErrorMessage = "Deleted column must be 0 or 1.")]
        public byte Deleted { get; set; }

        [Required]
        [Range(1, ulong.MaxValue, ErrorMessage = "End User ID must be greater than 0.")]
        public ulong Deleted_by { get; set; }

        [Timestamp_Is_Today_Or_Later]
        public ulong Created_on { get; set; }
        [Timestamp_Is_Today_Or_Later]
        public ulong Updated_on { get; set; }
        [Timestamp_Is_Today_Or_Later]
        public ulong Updated_by { get; set; }
        [Timestamp_Is_Today_Or_Later]
        public ulong Deleted_on { get; set; }
    }
}
