using mpc_dotnetc_user_server.Models.Users.Authentication.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Authentication.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Reported;
using mpc_dotnetc_user_server.Models.Users.Authentication.WebSocket_Chat;
using mpc_dotnetc_user_server.Models.Users.BirthDate;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Notification.Email;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public interface IUsersRepository
    {
        Task<string> Create_Reported_WebSocket_Abuse_Record(Reported_WebSocket_AbuseDTO dto);
        Task<bool> Create_Comment_Box_Record(DTO dto);
        Task<bool> Create_Broken_Link_Record(DTO dto);
        Task<bool> Create_Discord_Bot_Bug_Record(DTO dto);
        Task<string> Create_Integration_Twitch_Record(DTO dto);
        Task<string> Create_WebSocket_Log_Record(Websocket_Chat_PermissionDTO dto);
        Task<bool> Create_Website_Bug_Record(DTO dto);
        Task<bool> Create_End_User_Status_Record(Selected_StatusDTO dto);
        Task<string> Update_End_User_Account_Roles(Account_RolesDTO dto);
        Task<string> Update_End_User_Account_Groups(Account_GroupsDTO dto);
        Task<bool> Create_Contact_Us_Record(DTO dto);
        Task<byte[]> Create_Salted_Hash_String(byte[] text, byte[] salt);
        Task<string> Create_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto);
        Task<string> Create_Reported_Email_Registration_Record(Reported_Email_RegistrationDTO dto);
        Task<string> Create_Unconfirmed_Phone(DTO dto);
        Task<string> Create_Account_By_Email(Complete_Email_RegistrationDTO dto);
        Task<string> Create_Account_By_Phone(DTO dto);
        Task<string> Create_Reported_User_Profile_Record(DTO dto);
        Task<string> Delete_Account_By_User_id(DTO dto);
        Task<string> Insert_End_User_Login_Time_Stamp_History(Login_Time_StampDTO dto);
        Task<string> Insert_Report_Email_RegistrationTbl(Report_Email_RegistrationDTO dto);
        Task<string> Read_User(ulong end_user_id);
        Task<string> Read_Users();
        Task<string> Read_User_Profile(DTO dto);
        Task<string> Read_WebSocket_Permission_Record(Websocket_Chat_PermissionDTO dto);
        Task<string> Read_End_User_Web_Socket_Data(ulong end_user_id);
        Task<byte> Read_End_User_Selected_Status(Selected_StatusDTO dto);
        Task<byte[]?> Read_User_Password_Hash_By_ID(ulong id);
        Task<ulong> Read_User_ID_By_Email_Address(string email_address);
        Task<string?> Read_User_Email_By_ID(ulong id);
        Task<string> Update_Unconfirmed_Phone(DTO dto);
        Task<string> Insert_Pending_Email_Registration_History_Record(Pending_Email_Registration_HistoryDTO dto);
        Task<string> Update_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto);
        Task<string> Update_End_User_Avatar(Selected_AvatarDTO dto);
        Task<string> Update_End_User_Name(Selected_NameDTO dto);
        Task<string> Update_End_User_Login_Time_Stamp(Login_Time_StampDTO dto);
        Task<string> Update_End_User_Logout(ulong user_id);
        Task<string> Update_End_User_Selected_Alignment(Selected_App_AlignmentDTO dto);
        Task<string> Update_End_User_Selected_TextAlignment(Selected_App_Text_AlignmentDTO dto);
        Task<string> Update_End_User_Selected_Language(Selected_LanguageDTO dto);
        Task<string> Update_End_User_Selected_Nav_Lock(Selected_Navbar_LockDTO dto);
        Task<string> Update_End_User_Selected_Status(Selected_StatusDTO dto);
        Task<string> Update_End_User_Selected_Theme(Selected_ThemeDTO dto);
        Task<string> Update_End_User_First_Name(IdentityDTO dto);
        Task<string> Update_End_User_Last_Name(IdentityDTO dto);
        Task<string> Update_End_User_Middle_Name(IdentityDTO dto);
        Task<string> Update_End_User_Maiden_Name(IdentityDTO dto);
        Task<string> Update_End_User_Ethnicity(IdentityDTO dto);
        Task<string> Update_End_User_Gender(IdentityDTO dto);
        Task<string> Update_End_User_Password(Login_PasswordDTO dto);
        Task<string> Update_Chat_Web_Socket_Permissions_Tbl(Websocket_Chat_PermissionTbl dto);
        Task<string> Update_End_User_Birth_Date(Birth_DateDTO dto);
        void Create_Chat_WebSocket_Log_Records(Websocket_Chat_PermissionDTO dto);
        bool Compare_Password_Byte_Arrays(byte[] array1, byte[] array2);
        Task<bool> Telephone_Exists_In_Login_Telephone_Tbl(string telephone_number);
        Task<bool> Phone_Exists_In_Telephone_Not_Confirmed_Tbl(string telephone_number);
        Task<bool> Email_Exists_In_Pending_Email_RegistrationTbl(string email_address);
        Task<bool> Email_Exists_In_Login_Email_AddressTbl(string email_address);
        Task<bool> ID_Exists_In_Users_Tbl(ulong id);
    }
    public class DTO
    {
        public ulong ID { get; set; }
        public ulong Target_ID { get; set; }
        public ulong Created_on { get; set; }
        public ulong Login_on { get; set; }
        public ulong Logout_on { get; set; }
        public ulong Reported_ID { get; set; }
        public string Avatar_title { get; set; } = string.Empty;
        public string Avatar_url_path { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Custom_lbl { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Display_name { get; set; } = string.Empty;
        public string Email_Address { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Reported_Reason { get; set; } = string.Empty;
        public string Subject_line { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;
        public byte Country { get; set; }
        public byte Online_status { get; set; }
    }
}
