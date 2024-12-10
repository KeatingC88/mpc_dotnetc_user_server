using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Authentication;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Integration;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Models.Users.BirthDate;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class UsersDBC : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User_IDsTbl> User_IDsTbl { get; set; } = null!;
        public DbSet<Completed_Email_RegistrationTbl> Completed_Email_RegistrationTbl { get; set; } = null!;
        public DbSet<Pending_Email_RegistrationTbl> Pending_Email_RegistrationTbl { get; set; } = null!;
        public DbSet<Completed_Telephone_RegistrationTbl> Completed_Telephone_RegistrationTbl { get; set; } = null!;
        public DbSet<Pending_Telephone_RegistrationTbl> Pending_Telephone_RegistrationTbl { get; set; } = null!;
        public DbSet<Contact_UsTbl> Contact_UsTbl { get; set; } = null!;
        public DbSet<Comment_BoxTbl> Comment_BoxTbl { get; set; } = null!;
        public DbSet<Login_PasswordTbl> Login_PasswordTbl { get; set; } = null!;
        public DbSet<Login_Email_AddressTbl> Login_Email_AddressTbl { get; set; } = null!;
        public DbSet<Login_TelephoneTbl> Login_TelephoneTbl { get; set; } = null!;
        public DbSet<Login_Time_StampTbl> Login_Time_StampTbl { get; set; } = null!;
        public DbSet<Logout_Time_StampTbl> Logout_Time_StampTbl { get; set; } = null!;
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
        public DbSet<Reported_Broken_LinkTbl> Reported_Broken_LinkTbl { get; set; } = null!;
        public DbSet<Reported_Discord_Bot_BugTbl> Reported_Discord_Bot_BugTbl { get; set; } = null!;
        public DbSet<Reported_ProfileTbl> Reported_ProfileTbl { get; set; } = null!;
        public DbSet<Reported_Website_BugTbl> Reported_Website_BugTbl { get; set; } = null!;
        public DbSet<Reported_WebSocket_AbuseTbl> Reported_WebSocket_AbuseTbl { get; set; } = null!;
        public DbSet<Websocket_Chat_PermissionTbl> Websocket_Chat_PermissionTbl { get; set; } = null!;

        public UsersDBC(DbContextOptions<UsersDBC> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
    }//DbContext    
}//NameSpace