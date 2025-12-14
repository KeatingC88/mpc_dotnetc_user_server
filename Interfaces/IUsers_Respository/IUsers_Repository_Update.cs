using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Friends;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Password_Change;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;

namespace mpc_dotnetc_user_server.Interfaces.IUsers_Respository
{
    public interface IUsers_Repository_Update
    {

        Task<string> Update_End_User_Account_Roles(Account_Role dto);
        Task<string> Update_End_User_Account_Groups(Account_Group dto);
        Task<string> Update_End_User_Account_Type(Account_Types dto);

        Task<string> Update_Pending_Email_Registration_Record(Pending_Email_Registration dto);

        Task<string> Update_End_User_Avatar(Selected_Avatar dto);
        Task<string> Update_End_User_Avatar_Title(Selected_Avatar_Title dto);
        Task<string> Update_End_User_Name(Selected_Name dto);

        Task<string> Update_End_User_Login_Time_Stamp(Login_Time_Stamp dto);
        Task<string> Update_End_User_Logout(Logout_Time_Stamp dto);

        Task<string> Update_End_User_Selected_Alignment(Selected_App_Alignment dto);
        Task<string> Update_End_User_Selected_Text_Alignment(Selected_App_Text_Alignment dto);
        Task<string> Update_End_User_Selected_Language(Selected_Language dto);
        Task<string> Update_End_User_Selected_Nav_Lock(Selected_Navbar_Lock dto);
        Task<string> Update_End_User_Selected_Status(Selected_Status dto);
        Task<string> Update_End_User_Selected_Theme(Selected_Theme dto);
        Task<string> Update_End_User_Selected_Grid_Type(Selected_App_Grid_Type dto);

        Task<string> Update_End_User_First_Name(Identities dto);
        Task<string> Update_End_User_Last_Name(Identities dto);
        Task<string> Update_End_User_Middle_Name(Identities dto);
        Task<string> Update_End_User_Maiden_Name(Identities dto);
        Task<string> Update_End_User_Ethnicity(Identities dto);
        Task<string> Update_End_User_Gender(Identities dto);
        Task<string> Update_End_User_Birth_Date(Identities dto);

        Task<string> Update_End_User_Password(Password_Change dto);

        Task<string> Update_Chat_Web_Socket_Permissions(WebSocket_Chat_Permission dto);
        Task<string> Update_Friend_Permissions(Friends_Permission dto);

        Task<string> Update_End_User_Card_Border_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Header_Font(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Body_Font(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Footer_Font(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Navigation_Menu_Font(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Button_Font(Selected_App_Custom_Design dto);

        Task<string> Update_End_User_Card_Header_Font_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Body_Font_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Footer_Font_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Navigation_Menu_Font_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Button_Font_Color(Selected_App_Custom_Design dto);

        Task<string> Update_End_User_Card_Header_Background_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Body_Background_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Card_Footer_Background_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Navigation_Menu_Background_Color(Selected_App_Custom_Design dto);
        Task<string> Update_End_User_Button_Background_Color(Selected_App_Custom_Design dto);

    }
}
