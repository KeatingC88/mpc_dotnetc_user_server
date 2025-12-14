using mpc_dotnetc_user_server.Models.Users.Selected.Deactivate;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;

namespace mpc_dotnetc_user_server.Interfaces.IUsers_Respository
{
    public interface IUsers_Repository_Delete
    {

        Task<string> Delete_Account_By_User_id(Delete_User dto);

        Task<string> Delete_From_Web_Socket_Chat_Permissions(WebSocket_Chat_Permission dto);

        Task<string> Delete_End_User_Selected_App_Custom_Design(Selected_App_Custom_Design dto);

    }
}
