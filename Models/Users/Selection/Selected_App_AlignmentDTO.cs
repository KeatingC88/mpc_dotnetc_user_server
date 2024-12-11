using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_App_AlignmentDTO
    {
        [Required]
        public ulong User_id { get; set; }

        [Required]
        public AlignmentType Alignment { get; set; }

        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;

    }
}
