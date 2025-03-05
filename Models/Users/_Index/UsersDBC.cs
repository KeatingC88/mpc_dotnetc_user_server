using Microsoft.EntityFrameworkCore;

using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Integration;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Profile;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Selected.Password_Change;
using mpc_dotnetc_user_server.Models.Users.WebSocket_Chat;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class UsersDBC : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<User_IDsTbl> User_IDsTbl { get; set; } = null!;
        public DbSet<Account_TypeTbl> Account_TypeTbl { get; set; } = null!;
        public DbSet<Selected_App_Grid_TypeTbl> Selected_App_Grid_TypeTbl { get; set; } = null!;
        public DbSet<Account_RolesTbl> Account_RolesTbl { get; set; } = null!;
        public DbSet<Account_GroupsTbl> Account_GroupsTbl { get; set; } = null!;
        public DbSet<Completed_Email_RegistrationTbl> Completed_Email_RegistrationTbl { get; set; } = null!;
        public DbSet<Pending_Email_RegistrationTbl> Pending_Email_RegistrationTbl { get; set; } = null!;
        public DbSet<Pending_Email_Registration_HistoryTbl> Pending_Email_Registration_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Email_RegistrationTbl> Report_Email_RegistrationTbl { get; set; } = null!;
        public DbSet<Report_Failed_Email_Login_HistoryTbl> Report_Failed_Email_Login_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Failed_Logout_HistoryTbl> Report_Failed_Logout_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Failed_Client_ID_HistoryTbl> Report_Failed_Client_ID_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Failed_Selected_HistoryTbl> Report_Failed_Selected_HistoryTbl { get; set; } = null!;
        public DbSet<Selected_App_Custom_DesignTbl> Selected_App_Custom_DesignTbl { get; set; } = null!;
        public DbSet<Report_Failed_Unregistered_Email_Login_HistoryTbl> Report_Failed_Unregistered_Email_Login_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Failed_JWT_HistoryTbl> Report_Failed_JWT_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Failed_User_Agent_HistoryTbl> Report_Failed_User_Agent_HistoryTbl { get; set; } = null!;
        public DbSet<Report_Failed_Pending_Email_Registration_HistoryTbl> Report_Failed_Pending_Email_Registration_HistoryTbl { get; set; } = null!;
        public DbSet<Contact_UsTbl> Contact_UsTbl { get; set; } = null!;
        public DbSet<Comment_BoxTbl> Comment_BoxTbl { get; set; } = null!;
        public DbSet<Password_ChangeTbl> Login_PasswordTbl { get; set; } = null!;
        public DbSet<Login_Email_AddressTbl> Login_Email_AddressTbl { get; set; } = null!;
        public DbSet<Login_Time_StampTbl> Login_Time_StampTbl { get; set; } = null!;
        public DbSet<Login_Time_Stamp_HistoryTbl> Login_Time_Stamp_HistoryTbl { get; set; } = null!;
        public DbSet<Logout_Time_StampTbl> Logout_Time_StampTbl { get; set; } = null!;
        public DbSet<Logout_Time_Stamp_HistoryTbl> Logout_Time_Stamp_HistoryTbl { get; set; } = null!;
        public DbSet<IdentityTbl> IdentityTbl { get; set; } = null!;
        public DbSet<Birth_DateTbl> Birth_DateTbl { get; set; } = null!;
        public DbSet<ProfilePageTbl> ProfilePageTbl { get; set; } = null!;
        public DbSet<Integration_TwitchTbl> Integration_TwitchTbl { get; set; } = null!;
        public DbSet<Selected_StatusTbl> Selected_StatusTbl { get; set; } = null!;
        public DbSet<Selected_LanguageTbl> Selected_LanguageTbl { get; set; } = null!;
        public DbSet<Selected_App_AlignmentTbl> Selected_App_AlignmentTbl { get; set; } = null!;
        public DbSet<Selected_App_Text_AlignmentTbl> Selected_App_Text_AlignmentTbl { get; set; } = null!;
        public DbSet<Selected_Navbar_LockTbl> Selected_Navbar_LockTbl { get; set; } = null!;
        public DbSet<Selected_ThemeTbl> Selected_ThemeTbl { get; set; } = null!;
        public DbSet<Selected_NameTbl> Selected_NameTbl { get; set; } = null!;
        public DbSet<Selected_AvatarTbl> Selected_AvatarTbl { get; set; } = null!;
        public DbSet<Selected_Avatar_TitleTbl> Selected_Avatar_TitleTbl { get; set; } = null!;
        public DbSet<Reported_Broken_LinkTbl> Reported_Broken_LinkTbl { get; set; } = null!;
        public DbSet<Reported_Discord_Bot_BugTbl> Reported_Discord_Bot_BugTbl { get; set; } = null!;
        public DbSet<Reported_ProfileTbl> Reported_ProfileTbl { get; set; } = null!;
        public DbSet<Reported_Website_BugTbl> Reported_Website_BugTbl { get; set; } = null!;
        public DbSet<Reported_WebSocketTbl> Reported_WebSocketTbl { get; set; } = null!;
        public DbSet<Reported_WebSocket_HistoryTbl> Reported_WebSocket_HistoryTbl { get; set; } = null!;
        public DbSet<WebSocket_Chat_PermissionTbl> WebSocket_Chat_PermissionTbl { get; set; } = null!;
        public UsersDBC(DbContextOptions<UsersDBC> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
    }   
}