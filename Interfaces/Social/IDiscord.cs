using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;

namespace mpc_dotnetc_user_server.Interfaces.Social
{
    public interface IDiscord
    {
        Task<string?> Authorization_Code_Flow_Access_Token(string discord_code);
        Task<string?> Get_Client_Credentials_Flow_Access_Token();
        Task<Discord_User_Response?> Get_User_Data(string discord_code);
    }
}