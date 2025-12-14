using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;

namespace mpc_dotnetc_user_server.Interfaces.IUsers_Respository
{
    public interface IUsers_Repository_Integrate
    {

        Task<User_Data_DTO> Integrate_Account_By_Email(Complete_Email_Registration dto);

        Task Integrate_Account_By_Twitch(Complete_Twitch_Integration dto);

        Task Integrate_Account_By_Discord(Complete_Discord_Integration dto);

    }
}
