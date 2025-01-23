using System.ComponentModel.DataAnnotations;
using mpc_dotnetc_user_server.Models.Users.Selection;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Validate_Email_AddressDTO
    {
        [Required]
        public string Email_Address { get; set; } = string.Empty;
        [Required]
        public string Language { get; set; } = string.Empty;
        [Required]
        public string Region { get; set; } = string.Empty;
        [Required]
        public string Location { get;set; } = string.Empty;
        [Required]
        public string Client_time { get; set; } = string.Empty;
        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

    }
}