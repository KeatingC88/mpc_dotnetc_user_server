using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users._Index
{
    public class Load_All_UsersDTO
    {
        [Required]
        public string ID { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Client_time { get; set; } = string.Empty;

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

        [Required]
        public string Account_type {  get; set; } = string.Empty;

        [Required]
        public string Login_type { get; set; } = string.Empty;
    }
}
