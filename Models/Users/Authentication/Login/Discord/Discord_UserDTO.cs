using System.Text.Json.Serialization;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord
{
    public class Discord_UserDTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("username")]
        public string? User_name { get; set; }

        [JsonPropertyName("global_name")]
        public string? Global_name { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }
    }
}