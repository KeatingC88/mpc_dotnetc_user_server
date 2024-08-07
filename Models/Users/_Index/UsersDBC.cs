using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Chat;
using mpc_dotnetc_user_server.Models.Users.Authentication;
using mpc_dotnetc_user_server.Models.Users.Confirmation;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Integration;
using mpc_dotnetc_user_server.Models.Users.Selections;
using Microsoft.Extensions.Configuration;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class UsersDBC : DbContext
    {
        private readonly IConfiguration _configuration;

        public DbSet<User_IDsTbl> User_IDsTbl { get; set; } = null!;
        public DbSet<Confirmed_EmailAddressTbl> Confirmed_EmailAddressTbl { get; set; } = null!;
        public DbSet<Confirmed_TelephoneTbl> Confirmed_TelephoneTbl { get; set; } = null!;
        public DbSet<Unconfirmed_EmailAddressTbl> Unconfirmed_EmailAddressTbl { get; set; } = null!;
        public DbSet<Unconfirmed_TelephoneTbl> Unconfirmed_TelephoneTbl { get; set; } = null!;
        public DbSet<Contact_UsTbl> Contact_UsTbl { get; set; } = null!;
        public DbSet<Comment_BoxTbl> Comment_BoxTbl { get; set; } = null!;
        public DbSet<Login_PasswordTbl> Login_PasswordTbl { get; set; } = null!;
        public DbSet<Login_EmailAddressTbl> Login_EmailAddressTbl { get; set; } = null!;
        public DbSet<Login_TelephoneTbl> Login_TelephoneTbl { get; set; } = null!;
        public DbSet<Login_TSTbl> Login_TSTbl { get; set; } = null!;
        public DbSet<Logout_TSTbl> Logout_TSTbl { get; set; } = null!;
        public DbSet<IdentityTbl> IdentityTbl { get; set; } = null!;
        public DbSet<ProfilePageTbl> ProfilePageTbl { get; set; } = null!;
        public DbSet<Integration_TwitchTbl> Integration_TwitchTbl { get; set; } = null!;
        public DbSet<Selected_StatusTbl> Selected_StatusTbl { get; set; } = null!;
        public DbSet<Selected_LanguageTbl> Selected_LanguageTbl { get; set; } = null!;
        public DbSet<Selected_AlignmentTbl> Selected_AlignmentTbl { get; set; } = null!;
        public DbSet<Selected_AppTextAlignmentTbl> Selected_AppTextAlignmentTbl { get; set; } = null!;
        public DbSet<Selected_NavbarLockTbl> Selected_NavbarLockTbl { get; set; } = null!;
        public DbSet<Selected_ThemeTbl> Selected_ThemeTbl { get; set; } = null!;
        public DbSet<Selected_NameTbl> Selected_NameTbl { get; set; } = null!;
        public DbSet<Selected_AvatarTbl> Selected_AvatarTbl { get; set; } = null!;
        public DbSet<Reported_BrokenLinkTbl> Reported_BrokenLinkTbl { get; set; } = null!;
        public DbSet<Reported_DiscordBotBugTbl> Reported_DiscordBotBugTbl { get; set; } = null!;
        public DbSet<Reported_ProfileLogTbl> Reported_ProfileLogTbl { get; set; } = null!;
        public DbSet<Reported_WebsiteBugTbl> Reported_WebsiteBugTbl { get; set; } = null!;
        public DbSet<Chat_WebSocketLogTbl> Chat_WebSocketLogTbl { get; set; } = null!;
        public DbSet<Chat_WebSocketDirectMessagesTbl> Chat_WebSocketDirectMessagesTbl { get; set; } = null!;

        public UsersDBC(DbContextOptions<UsersDBC> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
    }//DbContext    
}//NameSpace