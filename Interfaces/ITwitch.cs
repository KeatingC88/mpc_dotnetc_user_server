using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;

namespace mpc_dotnetc_user_server.Interfaces
{
    public interface ITwitch
    {
        Task<string?> Get_Access_Token(string twitch_code);
        Task<Twitch_User_Response?> Get_User_Data(string twitch_code);
    }
}