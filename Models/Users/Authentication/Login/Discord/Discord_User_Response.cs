using System.Text.Json.Serialization;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord
{
    public class Discord_User_Response
    {
        [JsonPropertyName("data")]
        public List<Discord_UserDTO>? Data { get; set; }
    }
}
