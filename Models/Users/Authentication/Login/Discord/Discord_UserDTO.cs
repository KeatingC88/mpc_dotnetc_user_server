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

        [JsonPropertyName("avatar")]
        public string? Avatar { get; set; }

        [JsonPropertyName("discriminator")]
        public string? Discriminator { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("verified")]
        public bool? Verified { get; set; }

        [JsonPropertyName("locale")]
        public string? Language_region { get; set; }

        [JsonPropertyName("accent_color")]
        public int? Accent_color { get; set; }

        [JsonPropertyName("banner")]
        public string? Banner { get; set; }

        [JsonPropertyName("banner_color")]
        public string? Banner_color { get; set; }

        [JsonPropertyName("public_flags")]
        public int? Public_flags { get; set; }

        [JsonPropertyName("flags")]
        public int? Flags { get; set; }

        [JsonPropertyName("premium_type")]
        public int? Premium_type { get; set; }

        [JsonPropertyName("mfa_enabled")]
        public bool? Mfa_enabled { get; set; }

        [JsonPropertyName("avatar_decoration_data")]
        public object? Avatar_decoration_data { get; set; }

        [JsonPropertyName("collectibles")]
        public object? Collectibles { get; set; }

        [JsonPropertyName("display_name_styles")]
        public object? Display_name_styles { get; set; }

        [JsonPropertyName("clan")]
        public object? Clan { get; set; }

        [JsonPropertyName("primary_guild")]
        public object? Primary_guild { get; set; }
    }
}
