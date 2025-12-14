using mpc_dotnetc_user_server.Models.Security.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;

namespace mpc_dotnetc_user_server.Interfaces.IUsers_Respository
{
    public interface IUsers_Repository_Read
    {
        Task<bool> Read_Confirmation_Code_Exists_In_Pending_Email_Address_Registration(string code);

        Task<bool> Read_Email_Exists_In_Login_Email_Address(string email_address);

        Task<bool> Read_Email_Exists_In_Twitch_Email_Address(string email_address);

        Task<bool> Read_Email_Exists_In_Discord_Email_Address(string email_address);

        Task<bool> Read_Email_Exists_In_Pending_Email_Registration(string email_address);

        Task<User_Data_DTO> Read_User_Data_By_ID(long end_user_id);
        Task<string> Read_User_Profile_By_ID(long end_user_id);

        Task<string> Read_WebSocket_Permission_Record_For_Both_End_Users(WebSocket_Chat_Permission dto);

        Task<string> Read_End_User_WebSocket_Sent_Chat_Requests(long end_user_id);
        Task<string> Read_End_User_WebSocket_Sent_Chat_Blocks(long end_user_id);
        Task<string> Read_End_User_WebSocket_Sent_Chat_Approvals(long end_user_id);
        Task<string> Read_End_User_WebSocket_Received_Chat_Requests(long end_user_id);
        Task<string> Read_End_User_WebSocket_Received_Chat_Blocks(long end_user_id);
        Task<string> Read_End_User_WebSocket_Received_Chat_Approvals(long end_user_id);
        Task<string> Read_End_User_Friend_Permissions_By_ID(long end_user_id);

        Task<byte> Read_End_User_Selected_Status(long user_id);
        Task<byte[]?> Read_User_Password_Hash_By_ID(long id);

        Task<long> Read_User_ID_By_Email_Address(string email_address);
        Task<string?> Read_User_Email_By_ID(long id);

        Task<long> Read_User_ID_By_Twitch_Account_ID(long twitch_id);
        Task<long> Read_User_ID_By_Twitch_Account_Email(string twitch_email);

        Task<long> Read_User_ID_By_Discord_Account_ID(long discord_id);
        Task<long> Read_User_ID_By_Discord_Account_Email(string discord_email);

        Task<User_Token_Data_DTO> Read_Require_Token_Data_By_ID(long end_user_id);

        Task<bool> Read_ID_Exists_In_Users_ID(long id);

        Task<bool> Read_ID_Exists_In_Twitch_IDs(long twitch_id);

        Task<bool> Read_ID_Exists_In_Discord_IDs(long discord_id);


    }
}
