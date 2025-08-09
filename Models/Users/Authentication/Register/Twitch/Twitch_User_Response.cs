using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Twitch;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Register.Twitch
{
    public class Twitch_User_Response
    {
        [JsonPropertyName("data")]
        public List<TwitchUser_DTO>? Data { get; set; }
    }
}
