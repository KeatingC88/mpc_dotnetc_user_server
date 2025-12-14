using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord
{
    public class Discord_User_Response
    {
        [JsonPropertyName("data")]
        public List<Discord_UserDTO>? Data { get; set; }
    }
}
