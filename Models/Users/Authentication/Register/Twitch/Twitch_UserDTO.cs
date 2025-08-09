using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Register.Twitch
{
    public class TwitchUser_DTO
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("login")]
        public string? Login { get; set; }

        [JsonPropertyName("display_name")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("profile_image_url")]
        public string? ProfileImageUrl { get; set; }
    }
}
