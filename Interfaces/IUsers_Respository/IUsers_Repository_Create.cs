using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Report;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;

namespace mpc_dotnetc_user_server.Interfaces.IUsers_Respository
{
    public interface IUsers_Repository_Create
    {
        Task<string> Create_Reported_Record(Reported dto);
        Task<bool> Create_Comment_Box_Record(Comment_Box dto);
        Task<bool> Create_Broken_Link_Record(Reported_Broken_Link dto);
        Task<bool> Create_Discord_Bot_Bug_Record(Reported_Discord_Bot_Bug dto);
        Task<string> Create_WebSocket_Log_Record(WebSocket_Chat_Permission dto);
        Task<bool> Create_Website_Bug_Record(Reported_Website_Bug dto);
        Task<bool> Create_End_User_Status_Record(Selected_Status dto);
        Task<bool> Create_Contact_Us_Record(Contact_Us dto);
        Task<string> Create_Pending_Email_Registration_Record(Pending_Email_Registration dto);
        Task<User_Data_DTO> Create_Account_By_Email(Complete_Email_Registration dto);
        Task<User_Data_DTO> Create_Account_By_Twitch(Complete_Twitch_Registeration dto);
        Task<User_Data_DTO> Create_Account_By_Discord(Complete_Discord_Registeration dto);
        Task<string> Create_Reported_User_Profile_Record(Reported_Profile dto);
        Task<string> Insert_End_User_Login_Time_Stamp_History(Login_Time_Stamp_History dto);
        Task<string> Insert_Report_Email_Registration(Report_Email_Registration dto);
        Task<string> Insert_Pending_Email_Registration_History_Record(Pending_Email_Registration_History dto);
        Task<string> Insert_Report_Failed_Email_Login_History_Record(Report_Failed_Email_Login_History dto);
        Task<string> Insert_Report_Failed_Logout_History(Report_Failed_Logout_History dto);
        Task<string> Insert_Report_Failed_Unregistered_Email_Login_History_Record(Report_Failed_Unregistered_Email_Login_History dto);
        Task<string> Insert_Report_Failed_Pending_Email_Registration_History(Report_Failed_Pending_Email_Registration_History dto);
        Task<string> Insert_Report_Failed_JWT_History_Record(Report_Failed_JWT_History dto);
        Task<string> Insert_Report_Failed_User_Agent_History(Report_Failed_User_Agent_History dto);
        Task<string> Insert_End_User_Logout_History_Record(Logout_Time_Stamp dto);
        Task<string> Insert_Report_Failed_Selected_History(Report_Failed_Selected_History dto);
        Task<string> Insert_Report_Failed_Client_ID_History_Record(Report_Failed_Client_ID_History dto);

    }
}
