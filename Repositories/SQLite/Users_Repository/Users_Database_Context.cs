using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Friends;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Profile;
using mpc_dotnetc_user_server.Models.Users.Report;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository
{
    public class Users_Database_Context : DbContext
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
        public DbSet<Login_PasswordTbl> Login_PasswordTbl { get; set; } = null!;
        public DbSet<Login_Email_AddressTbl> Login_Email_AddressTbl { get; set; } = null!;
        public DbSet<Login_Time_StampTbl> Login_Time_StampTbl { get; set; } = null!;
        public DbSet<Login_Time_Stamp_HistoryTbl> Login_Time_Stamp_HistoryTbl { get; set; } = null!;
        public DbSet<Logout_Time_StampTbl> Logout_Time_StampTbl { get; set; } = null!;
        public DbSet<Logout_Time_Stamp_HistoryTbl> Logout_Time_Stamp_HistoryTbl { get; set; } = null!;
        public DbSet<IdentityTbl> IdentityTbl { get; set; } = null!;
        public DbSet<Birth_DateTbl> Birth_DateTbl { get; set; } = null!;
        public DbSet<Profile_PageTbl> Profile_PageTbl { get; set; } = null!;
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
        public DbSet<ReportedTbl> ReportedTbl { get; set; } = null!;
        public DbSet<Reported_ReasonTbl> Reported_ReasonTbl { get; set; } = null!;
        public DbSet<Reported_HistoryTbl> Reported_HistoryTbl { get; set; } = null!;
        public DbSet<WebSocket_Chat_PermissionTbl> WebSocket_Chat_PermissionTbl { get; set; } = null!;
        public DbSet<Friends_PermissionTbl> Friends_PermissionTbl { get; set; } = null!;
        public DbSet<Twitch_Email_AddressTbl> Twitch_Email_AddressTbl { get;set; } = null!;
        public DbSet<Twitch_IDsTbl> Twitch_IDsTbl { get;set; } = null!;
        public DbSet<Discord_Email_AddressTbl> Discord_Email_AddressTbl { get; set; } = null!;
        public DbSet<Discord_IDsTbl> Discord_IDsTbl { get; set; } = null!;
        public Users_Database_Context(DbContextOptions<Users_Database_Context> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User_IDsTbl>(entity =>
            {
                entity.ToTable("User_IDsTbl");
                entity.HasKey(e => e.ID);
                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Public_ID).IsRequired();
                entity.Property(e => e.Secret_Hash_ID).IsRequired();
                entity.Property(e => e.Secret_ID).IsRequired();
            });

            modelBuilder.Entity<Account_TypeTbl>(entity =>
            {
                entity.ToTable("Account_TypeTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Type).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Account_RolesTbl>(entity =>
            {
                entity.ToTable("Account_RolesTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Roles).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Account_GroupsTbl>(entity =>
            {
                entity.ToTable("Account_GroupsTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Groups).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Comment_BoxTbl>(entity =>
            {
                entity.ToTable("Comment_BoxTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Comment).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Contact_UsTbl>(entity =>
            {
                entity.ToTable("Contact_UsTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Subject_Line).IsRequired();
                entity.Property(e => e.Summary).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Login_PasswordTbl>(entity =>
            {
                entity.ToTable("Login_PasswordTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();

                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Password).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Login_Time_StampTbl>(entity =>
            {
                entity.ToTable("Login_Time_StampTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.Property(e => e.Login_on).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();

                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();

                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();

                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Login_Time_Stamp_HistoryTbl>(entity =>
            {
                entity.ToTable("Login_Time_Stamp_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.Property(e => e.Login_on).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();

                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();

                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();

                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Logout_Time_StampTbl>(entity =>
            {
                entity.ToTable("Logout_Time_StampTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.Property(e => e.Logout_on).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.Location).IsRequired();

                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();

                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();

                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Logout_Time_StampTbl>(entity =>
            {
                entity.ToTable("Logout_Time_StampTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.Property(e => e.Logout_on).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.Location).IsRequired();

                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();

                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();

                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Login_Email_AddressTbl>(entity =>
            {
                entity.ToTable("Login_Email_AddressTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report_Email_RegistrationTbl>(entity =>
            {
                entity.ToTable("Report_Email_RegistrationTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Report_Failed_JWT_HistoryTbl>(entity =>
            {
                entity.ToTable("Report_Failed_JWT_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Login_type).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Controller).IsRequired();
                entity.Property(e => e.Action).IsRequired();
                entity.Property(e => e.Client_id).IsRequired();
                entity.Property(e => e.JWT_id).IsRequired();
                entity.Property(e => e.JWT_client_address).IsRequired();
                entity.Property(e => e.JWT_client_key).IsRequired();
                entity.Property(e => e.JWT_issuer_key).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Reason).IsRequired();
                entity.Property(e => e.Token).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Report_Failed_Client_ID_HistoryTbl>(entity =>
            {
                entity.ToTable("Report_Failed_Client_ID_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Controller).IsRequired();
                entity.Property(e => e.Action).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Report_Failed_Pending_Email_Registration_HistoryTbl>(entity =>
            {
                entity.ToTable("Report_Failed_Pending_Email_Registration_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Action).IsRequired();
                entity.Property(e => e.Controller).IsRequired();
                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Report_Failed_Email_Login_HistoryTbl>(entity =>
            {
                entity.ToTable("Report_Failed_Email_Login_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report_Failed_Email_Logout_HistoryTbl>(entity =>
            {
                entity.ToTable("Report_Failed_Email_Logout_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Report_Failed_Selected_HistoryTbl>(entity =>
            {
                entity.ToTable("Report_Failed_Selected_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Reason).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Pending_Email_RegistrationTbl>(entity =>
            {
                entity.ToTable("Pending_Email_RegistrationTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Code).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Pending_Email_Registration_HistoryTbl>(entity =>
            {
                entity.ToTable("Pending_Email_Registration_HistoryTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Code).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Completed_Email_RegistrationTbl>(entity =>
            {
                entity.ToTable("Completed_Email_RegistrationTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.Language_Region).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Code).IsRequired();
                entity.Property(e => e.Remote_IP).IsRequired();
                entity.Property(e => e.Remote_Port).IsRequired();
                entity.Property(e => e.Server_IP).IsRequired();
                entity.Property(e => e.Server_Port).IsRequired();
                entity.Property(e => e.Client_IP).IsRequired();
                entity.Property(e => e.Client_Port).IsRequired();
                entity.Property(e => e.Client_time).IsRequired();
                entity.Property(e => e.User_agent).IsRequired();
                entity.Property(e => e.Down_link).IsRequired();
                entity.Property(e => e.Connection_type).IsRequired();
                entity.Property(e => e.RTT).IsRequired();
                entity.Property(e => e.Data_saver).IsRequired();
                entity.Property(e => e.Device_ram_gb).IsRequired();
                entity.Property(e => e.Orientation).IsRequired();
                entity.Property(e => e.Screen_width).IsRequired();
                entity.Property(e => e.Screen_height).IsRequired();
                entity.Property(e => e.Color_depth).IsRequired();
                entity.Property(e => e.Pixel_depth).IsRequired();
                entity.Property(e => e.Window_width).IsRequired();
                entity.Property(e => e.Window_height).IsRequired();
                entity.Property(e => e.Location).IsRequired();

                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Selected_App_Custom_DesignTbl>(entity =>
            {
                entity.ToTable("Selected_App_Custom_DesignTbl");
                entity.HasKey(e => e.ID);

                entity.Property(e => e.ID).ValueGeneratedOnAdd();
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Card_Border_Color).IsRequired();
                entity.Property(e => e.Card_Header_Font).IsRequired();
                entity.Property(e => e.Card_Header_Background_Color).IsRequired();
                entity.Property(e => e.Card_Header_Font_Color).IsRequired();
                entity.Property(e => e.Card_Body_Font).IsRequired();
                entity.Property(e => e.Card_Body_Background_Color).IsRequired();
                entity.Property(e => e.Card_Body_Font_Color).IsRequired();
                entity.Property(e => e.Card_Footer_Font).IsRequired();
                entity.Property(e => e.Card_Footer_Background_Color).IsRequired();
                entity.Property(e => e.Card_Footer_Font_Color).IsRequired();
                entity.Property(e => e.Navigation_Menu_Background_Color).IsRequired();
                entity.Property(e => e.Navigation_Menu_Font_Color).IsRequired();
                entity.Property(e => e.Navigation_Menu_Font).IsRequired();
                entity.Property(e => e.Button_Background_Color).IsRequired();
                entity.Property(e => e.Button_Font_Color).IsRequired();
                entity.Property(e => e.Button_Font).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();

                entity.HasOne<User_IDsTbl>()
                      .WithMany()
                      .HasForeignKey(e => e.End_User_ID)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Selected_Navbar_LockTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Locked).IsRequired();
            });

            modelBuilder.Entity<Selected_StatusTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Offline).IsRequired();
                entity.Property(e => e.Hidden).IsRequired();
                entity.Property(e => e.Online).IsRequired();
                entity.Property(e => e.Away).IsRequired();
                entity.Property(e => e.DND).IsRequired();
                entity.Property(e => e.Custom).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Custom_lbl);
            });

            modelBuilder.Entity<Selected_ThemeTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Light).IsRequired();
                entity.Property(e => e.Night).IsRequired();
                entity.Property(e => e.Custom).IsRequired();
            });

            modelBuilder.Entity<Reported_Website_BugTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.URL).IsRequired();
                entity.Property(e => e.Detail).IsRequired();
            });

            modelBuilder.Entity<Reported_Discord_Bot_BugTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Location).IsRequired();
                entity.Property(e => e.Detail).IsRequired();
            });

            modelBuilder.Entity<Reported_Broken_LinkTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.URL).IsRequired();
            });

            modelBuilder.Entity<ReportedTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Spam).IsRequired();
                entity.Property(e => e.Abuse).IsRequired();
                entity.Property(e => e.Block).IsRequired();
                entity.Property(e => e.Fake).IsRequired();
                entity.Property(e => e.Hate).IsRequired();
                entity.Property(e => e.Nudity).IsRequired();
                entity.Property(e => e.Violence).IsRequired();
                entity.Property(e => e.Threat).IsRequired();
                entity.Property(e => e.Misinform).IsRequired();
                entity.Property(e => e.Harass).IsRequired();
                entity.Property(e => e.Illegal).IsRequired();
                entity.Property(e => e.Self_harm).IsRequired();
                entity.Property(e => e.Disruption).IsRequired();
            });

            modelBuilder.Entity<IdentityTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Ethnicity).IsRequired();
                entity.Property(e => e.Gender).IsRequired();
                entity.Property(e => e.First_Name).IsRequired();
                entity.Property(e => e.Middle_Name).IsRequired();
                entity.Property(e => e.Last_Name).IsRequired();
                entity.Property(e => e.Maiden_Name).IsRequired();
            });

            modelBuilder.Entity<Birth_DateTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
                entity.Property(e => e.Month).IsRequired();
                entity.Property(e => e.Day).IsRequired();
                entity.Property(e => e.Year).IsRequired();
            });

            modelBuilder.Entity<Twitch_IDsTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Twitch_ID).IsRequired();
                entity.Property(e => e.User_Name).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Discord_IDsTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Discord_ID).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Twitch_Email_AddressTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });

            modelBuilder.Entity<Discord_Email_AddressTbl>(entity =>
            {
                entity.Property(e => e.End_User_ID).IsRequired();
                entity.Property(e => e.Email_Address).IsRequired();
                entity.Property(e => e.Created_by).IsRequired();
                entity.Property(e => e.Created_on).IsRequired();
                entity.Property(e => e.Deleted).IsRequired();
                entity.Property(e => e.Deleted_on).IsRequired();
                entity.Property(e => e.Deleted_by).IsRequired();
                entity.Property(e => e.Updated_on).IsRequired();
                entity.Property(e => e.Updated_by).IsRequired();
            });
        }
    }
}