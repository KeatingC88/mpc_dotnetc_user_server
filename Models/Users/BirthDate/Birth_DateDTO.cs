using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.BirthDate
{
    public class Birth_DateDTO
    {
        public ulong User_id { get; set; }
        public byte Month { get; set; }
        public byte Day { get; set; }
        public ulong Year { get; set; }
        [Required(ErrorMessage = "Application Token is Missing.")]
        [StringLength(int.MaxValue, MinimumLength = 8, ErrorMessage = "Application Token must equal greater than 3.")]
        public string Token { get; set; } = string.Empty;
    }
}