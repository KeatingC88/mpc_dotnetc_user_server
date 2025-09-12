using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;

namespace mpc_dotnetc_user_server.Interfaces
{
    public interface ITwitch
    {
        Task<string?> Authorization_Code_Flow_Access_Token(string twitch_code);
        Task<string?> Get_Client_Credentials_Flow_Access_Token();
        Task<Twitch_User_Response?> Get_User_Data(string twitch_code);
    }
}