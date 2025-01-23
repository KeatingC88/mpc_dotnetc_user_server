using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Alignment
{
    public class Selected_App_Grid_TypeDTO
    {
        [Required(ErrorMessage = "Grid Type is Missing.")]
        public byte Grid { get; set; }

        [Required(ErrorMessage = "Application Token is Missing.")]
        public string Token { get; set; } = string.Empty;

        public ulong User_id { get; set; }
    }
}
