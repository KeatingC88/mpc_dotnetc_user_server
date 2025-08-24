using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch
{
    public class Twitch_User_Response
    {
        [JsonPropertyName("data")]
        public List<TwitchUser_DTO>? Data { get; set; }
    }
}
