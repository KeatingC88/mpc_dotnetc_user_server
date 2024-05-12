using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using dotnet_user_server.Models.Users.Chat;
using dotnet_user_server.Models.Users.Authentication;
using dotnet_user_server.Models.Users.Confirmation;
using dotnet_user_server.Models.Users.Feedback;
using dotnet_user_server.Models.Users.Identity;
using dotnet_user_server.Models.Users.Integration;
using dotnet_user_server.Models.Users.Selections;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace dotnet_user_server.Models.Users
{
    public class UsersDbC : DbContext
    {
        private readonly IConfiguration _configuration;
        private readonly int TokenExpireTime = 9999;//JWT expire in Minutes.
        private readonly ulong TimeStamp = Convert.ToUInt64(DateTimeOffset.Now.ToUnixTimeSeconds());
        private dynamic obj = new ExpandoObject();
        //Database Table Models
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
        public UsersDbC(DbContextOptions<UsersDbC> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }
        //Database Explicit Transaction Methods.
        public ActionResult<string> Create_Unconfirmed_Email(DTO dto)
        {
            Unconfirmed_EmailAddressTbl.AddAsync(new Unconfirmed_EmailAddressTbl
            {
                ID = Convert.ToUInt64(Unconfirmed_EmailAddressTbl.Count() + 1),
                Email = dto.Email_address,
                Code = dto.Code,
                Language_Region = dto.Language,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();
            obj.email_address = dto.Email_address;
            obj.code = dto.Code;
            obj.language = dto.Language;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Create_Unconfirmed_Phone(DTO dto)
        {
            Unconfirmed_TelephoneTbl.AddAsync(new Unconfirmed_TelephoneTbl
            {
                ID = Convert.ToUInt64(Unconfirmed_TelephoneTbl.Count() + 1),
                Country = dto.Country,
                Phone = dto.Phone,
                Code = dto.Code,
                Carrier = dto.Carrier,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();

            obj.phone = dto.Phone;
            obj.country = dto.Country;
            obj.carrier = dto.Carrier;
            obj.code = dto.Code;

            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Create_Account_By_Email(DTO dto)
        {
            Unconfirmed_EmailAddressTbl.Where(x => x.Email == dto.Email_address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, 0F)
                .SetProperty(col => col.Deleted_by, 0F)
            );
            SaveChangesAsync();

            Confirmed_EmailAddressTbl.AddAsync(new Confirmed_EmailAddressTbl
            {
                Email = dto.Email_address,
                Updated_on = TimeStamp,
                Updated_by = 0,
                Language_Region = dto.Language,
                Created_on = TimeStamp,
            });
            SaveChangesAsync();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(User_IDsTbl.Count() + 1),
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            };
            User_IDsTbl.AddAsync(ID_Record);
            SaveChangesAsync();

            Login_EmailAddressTbl.AddAsync(new Login_EmailAddressTbl
            {
                ID = Convert.ToUInt64(Login_EmailAddressTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Email = dto.Email_address,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();

            Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                ID = Convert.ToUInt64(Login_PasswordTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Password = UsersDbC.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_address}MPCSalt")),
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();

            Login_TSTbl.AddAsync(new Login_TSTbl
            {
                ID = Convert.ToUInt64(Login_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Login_on = TimeStamp,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();

            Logout_TSTbl.AddAsync(new Logout_TSTbl
            {
                ID = Convert.ToUInt64(Logout_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Logout_on = 0,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();
            
            obj.id = ID_Record.ID;
            obj.email_address = dto.Email_address;
            obj.token = Create_Jwt_Token(@$"{ID_Record.ID}");
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.created_on = TimeStamp;
            obj.language = dto.Language;
            obj.alignment = dto.Alignment;
            obj.text_alignment = dto.Text_alignment;
            obj.nav_lock = dto.Nav_lock;
            obj.theme = dto.Theme;
            obj.online_status = 1;
            obj.name = dto.Email_address;
            obj.display_name = dto.Email_address;

            Create_EndUser_Database_Fields(obj);
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Create_Account_By_Phone(DTO dto)
        {
            Unconfirmed_TelephoneTbl.Where(x => x.Phone == dto.Phone).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );

            Confirmed_TelephoneTbl.AddAsync(new Confirmed_TelephoneTbl
            {
                Country = dto.Country,
                Phone = dto.Phone,
                Code = dto.Code,
                Carrier = dto.Carrier,
                Updated_on = TimeStamp
            });
            SaveChangesAsync();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(User_IDsTbl.Count() + 1),
                Created_on = TimeStamp
            };
            User_IDsTbl.AddAsync(ID_Record);
            SaveChangesAsync();

            Login_TelephoneTbl.AddAsync(new Login_TelephoneTbl
            {
                ID = Convert.ToUInt64(Login_TelephoneTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Phone = dto.Phone,
                Country = dto.Country,
                Carrier = dto.Carrier,
                Created_on = TimeStamp,
                Updated_by = ID_Record.ID
            });
            SaveChangesAsync();

            Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                ID = Convert.ToUInt64(Login_PasswordTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Password = UsersDbC.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Phone}MPCSalt")),
                Created_on = TimeStamp
            });
            SaveChangesAsync();

            Login_TSTbl.AddAsync(new Login_TSTbl
            {
                ID = Convert.ToUInt64(Login_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Login_on = TimeStamp,
            });
            SaveChangesAsync();

            Logout_TSTbl.AddAsync(new Logout_TSTbl
            {
                ID = Convert.ToUInt64(Logout_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Logout_on = 0,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            SaveChangesAsync();

            obj.id = ID_Record.ID;
            obj.token = Create_Jwt_Token(@$"{ID_Record.ID}");
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.created_on = TimeStamp;
            obj.language = dto.Language;
            obj.alignment = dto.Alignment;
            obj.text_alignment = dto.Text_alignment;
            obj.nav_lock = dto.Nav_lock;
            obj.theme = dto.Theme;
            obj.online_status = 1;
            obj.name = $"New User: {ID_Record.ID}";
            obj.display_name = $"New User: {ID_Record.ID}";
            obj.phone = dto.Phone;
            obj.country = dto.Country;
            obj.carrier = dto.Carrier;

            Create_EndUser_Database_Fields(obj);
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<bool> Create_Contact_Us_Record(Contact_UsDTO obj)
        {
            Contact_UsTbl.AddAsync(new Contact_UsTbl
            {
                ID = Convert.ToUInt64(Contact_UsTbl.Count() + 1),
                USER_ID = obj.ID,
                Subject_Line = obj.Subject_Line,
                Summary = obj.Summary,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<bool> Create_Website_Bug_Record(Reported_WebsiteBugDTO obj) 
        {
            Reported_WebsiteBugTbl.AddAsync(new Reported_WebsiteBugTbl
            {
                ID = Convert.ToUInt64(Reported_WebsiteBugTbl.Count() + 1),
                USER_ID = obj.ID,
                URL = obj.URL,
                Detail = obj.Detail,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<bool> Create_WebSocket_DM_Record(Chat_WebSocketDirectMessagesDTO obj) 
        {
            Chat_WebSocketDirectMessagesTbl.AddAsync(new Chat_WebSocketDirectMessagesTbl
            {
                ID = Convert.ToUInt64(Chat_WebSocketDirectMessagesTbl.Count() + 1),
                User_id = obj.ID,
                Sent_to = obj.Sent_to,
                Message = obj.Message,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                HostClient_ts = obj.timestamp,
                Updated_by = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<bool> Create_WebSocket_Log_Record(Chat_WebSocketLogDTO obj)
        {
            Chat_WebSocketLogTbl.AddAsync(new Chat_WebSocketLogTbl
            {
                ID = Convert.ToUInt64(Chat_WebSocketLogTbl.Count() + 1),
                User_id = obj.ID,
                Sent_to = obj.Send_to,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
                Requested = 1,
                Approved = 0,
                Blocked = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<bool> Create_Discord_Bot_Bug_Record(Discord_Bot_BugDTO obj)
        {
            Reported_DiscordBotBugTbl.AddAsync(new Reported_DiscordBotBugTbl
            {
                ID = Convert.ToUInt64(Reported_DiscordBotBugTbl.Count() + 1),
                USER_ID = obj.ID,
                Location = obj.Location,
                Detail = obj.Detail,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<bool> Create_Comment_Box_Record(Comment_BoxDTO obj)
        {
            Comment_BoxTbl.AddAsync(new Comment_BoxTbl
            {
                ID = Convert.ToUInt64(Comment_BoxTbl.Count() + 1),
                USER_ID = obj.ID,
                Comment = obj.Comment,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<bool> Create_Broken_Link_Record(Broken_LinkDTO obj)
        {
            Reported_BrokenLinkTbl.AddAsync(new Reported_BrokenLinkTbl
            {
                ID = Convert.ToUInt64(Reported_BrokenLinkTbl.Count() + 1),
                USER_ID = obj.ID,
                URL = obj.URL,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            SaveChangesAsync();
            return true;
        }
        public ActionResult<string> Create_Reported_User_Record(Reported_DTO dto)
        {
            Reported_ProfileLogTbl record = new Reported_ProfileLogTbl
            {
                ID = Convert.ToUInt64(Reported_ProfileLogTbl.Count() + 1),
                USER_ID = dto.ID,
                Reported_ID = dto.Reported_ID,
                Page_Title = ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Page_Title).SingleOrDefault(),
                Page_Description = ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Page_Description).SingleOrDefault(),
                About_Me = ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.About_Me).SingleOrDefault(),
                Banner_URL = ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Banner_URL).SingleOrDefault(),
                Reported_Reason = dto.Reported_Reason,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.ID
            };
            Reported_ProfileLogTbl.AddAsync(record);
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.report_record_id = record.ID;
            obj.reported_user_id = record.Reported_ID;
            obj.created_on = record.Created_on;
            obj.read_reported_user = Read_User(new DTO{ID = dto.Reported_ID}).ToString();
            obj.read_reported_profile = Read_User_Profile(new DTO{ID = dto.Reported_ID}).ToString();
            return JsonSerializer.Serialize(obj);
        }
        public void Create_Chat_WebSocket_Log_Records(Chat_WebSocketLogDTO dto) 
        {
            Chat_WebSocketLogTbl.AddAsync(new Chat_WebSocketLogTbl
            {
                ID = Convert.ToUInt64(Chat_WebSocketLogTbl.Count() + 1),
                User_id = dto.ID,
                Sent_to = dto.Send_to,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.ID,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            SaveChangesAsync();
            Chat_WebSocketLogTbl.AddAsync(new Chat_WebSocketLogTbl
            {
                ID = Convert.ToUInt64(Chat_WebSocketLogTbl.Count() + 1),
                User_id = dto.Send_to,//Swapped so we are to create the record for the other user.
                Sent_to = dto.ID,//Ditto.
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.ID,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            SaveChangesAsync();
        }
        public ActionResult<string> Delete_Account_By_User_ID(DTO dto)
        {
            User_IDsTbl.Where(x => x.ID == dto.Target_id).ExecuteUpdateAsync(s => s
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted, 1)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_by, dto.ID)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Created_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_by, dto.ID)
            );
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.Target_id = dto.Target_id;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Read_User(DTO dto)
        {//Getting Information About the End User...
            bool Nav_lock = Selected_NavbarLockTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Locked).SingleOrDefault();
            byte status_online = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            byte Status = 0;
            byte Light = Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Light).SingleOrDefault();
            byte Night = Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Night).SingleOrDefault();
            byte CustomTheme = Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            byte Theme = 0;
            byte LeftAligned = Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Left).SingleOrDefault();
            byte CenterAligned = Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Center).SingleOrDefault();
            byte RightAligned = Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Right).SingleOrDefault();
            byte Alignment = 0;
            byte LeftTextAligned = Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Left).SingleOrDefault();
            byte CenterTextAligned = Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Center).SingleOrDefault();
            byte RightTextAligned = Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Right).SingleOrDefault();
            byte TextAlignment = 0;
            ulong LoginTS = Login_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = Logout_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Logout_on).SingleOrDefault();
            ulong Created_on = User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();
            string? customLbl = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom_lbl).SingleOrDefault();
            string? Email = Login_EmailAddressTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Email).SingleOrDefault();
            string? RegionCode = Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? Avatar_url_path = Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? Avatar_title = Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_title).SingleOrDefault();
            string? DisplayName = Selected_NameTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Name).SingleOrDefault();
            byte? Gender = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Gender).SingleOrDefault();
            byte? BirthDay = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.B_Day).SingleOrDefault();
            byte? BirthMonth = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.B_Month).SingleOrDefault();
            ulong? BirthYear = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.B_Year).SingleOrDefault();
            string? FirstName = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.First_Name).SingleOrDefault();
            string? LastName = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Last_Name).SingleOrDefault();
            string? MiddleName = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Middle_Name).SingleOrDefault();
            string? MaidenName = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Maiden_Name).SingleOrDefault();
            string? Ethnicity = IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Ethnicity).SingleOrDefault();

            if (status_offline == 1)
                Status = 0;
            if (status_hidden == 1)
                Status = 1;
            if (status_online == 1)
                Status = 2;
            if (status_away == 1)
                Status = 3;
            if (status_dnd == 1)
                Status = 4;
            if (status_custom == 1)
                Status = 5;
            if (Light == 1)
                Theme = 0;
            if (Night == 1)
                Theme = 1;
            if (CustomTheme == 1)
                Theme = 2;
            if (LeftAligned == 1)
                Alignment = 0;
            if (CenterAligned == 1)
                Alignment = 1;
            if (RightAligned == 1)
                Alignment = 2;
            if (LeftTextAligned == 1)
                TextAlignment = 0;
            if (CenterTextAligned == 1)
                TextAlignment = 1;
            if (RightTextAligned == 1)
                TextAlignment = 2;

            obj.id = dto.ID;
            obj.email_address = Email;
            obj.display_name = DisplayName;
            obj.login_on = LoginTS;
            obj.logout_on = LogoutTS;
            obj.token = dto.Token;
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.language = @$"{LanguageCode}-{RegionCode}";
            obj.online_status = Status;
            obj.custom_lbl = customLbl;
            obj.created_on = Created_on;
            obj.avatar_url_path = Avatar_url_path;
            obj.avatar_title = Avatar_title;
            obj.theme = Theme;
            obj.alignment = Alignment;
            obj.text_alignment = TextAlignment;
            obj.gender = Gender;
            obj.birth_day = BirthDay;
            obj.birth_month = BirthMonth;
            obj.birth_year = BirthYear;
            obj.first_name = FirstName;
            obj.last_name = LastName;
            obj.middle_name = MiddleName;
            obj.maiden_name = MaidenName;
            obj.ethnicity = Ethnicity;

            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Read_User_Profile(DTO dto)
        {
            //Get Information About the End User for the client side.
            byte status_online = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            string? customLbl = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom_lbl).SingleOrDefault();

            //Send Information to Client Side going below...
            byte Status = 0;

            ulong LoginTS = Login_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = Logout_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Logout_on).SingleOrDefault();
            ulong Created_on = User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();

            string? Email = Login_EmailAddressTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Email).SingleOrDefault();
            string? RegionCode = Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? Avatar_url_path = Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? Avatar_title = Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_title).SingleOrDefault();
            string? DisplayName = Selected_NameTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Name).SingleOrDefault();

            if (status_offline == 1)
                Status = 0;
            if (status_hidden == 1)
                Status = 1;
            if (status_online == 1)
                Status = 2;
            if (status_away == 1)
                Status = 3;
            if (status_dnd == 1)
                Status = 4;
            if (status_custom == 1)
                Status = 5;

            return JsonSerializer.Serialize(new DTO
            {
                ID = dto.ID,
                Email_address = Email,
                Display_name = DisplayName,
                Login_on = LoginTS,
                Logout_on = LogoutTS,
                Language = @$"{LanguageCode}-{RegionCode}",
                Online_status = Status,
                Custom_lbl = customLbl,
                Created_on = Created_on,
                Avatar_url_path = Avatar_url_path,
                Avatar_title = Avatar_title
            });
        }
        public ActionResult<string> Read_Chat_Web_Socket_Log_Record(Chat_WebSocketLogDTO dto)
        {
            byte requested = Chat_WebSocketLogTbl.Where(x => x.User_id == dto.ID && x.Sent_to == dto.Send_to).Select(x => x.Requested).SingleOrDefault();
            byte approved = Chat_WebSocketLogTbl.Where(x => x.User_id == dto.ID && x.Sent_to == dto.Send_to).Select(x => x.Approved).SingleOrDefault();
            byte blocked = Chat_WebSocketLogTbl.Where(x => x.User_id == dto.ID && x.Sent_to == dto.Send_to).Select(x => x.Blocked).SingleOrDefault();

            if (requested == 1)
            {
                obj.requested = 1;
                obj.blocked = 0;
                obj.approved = 0;
            }

            if (approved == 1)
            {
                obj.requested = 0;
                obj.blocked = 0;
                obj.approved = 1;
            }

            if (blocked == 1) {
                obj.requested = 0;
                obj.blocked = 1;
                obj.approved = 0;
            }

            if (requested == 0 && approved == 0 && blocked == 0)
            {//When no records found, where-as, the current user and other end user have engaged with chat for the first time.
                Create_Chat_WebSocket_Log_Records(dto);
                obj.requested = 1;
                obj.blocked = 0;
                obj.approved = 0;
            }

            obj.id = dto.ID;
            obj.send_to = dto.Send_to;
            obj.message = dto.Message;

            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Read_End_User_Web_Socket_Data(Chat_WebSocketLogDTO dto)
        {
            return JsonSerializer.Serialize(Chat_WebSocketLogTbl.Where(x => x.User_id == dto.ID).ToList().Concat(Chat_WebSocketLogTbl.Where(x => x.Sent_to == dto.ID).ToList()));
        }
        public byte Read_End_User_Selected_Status(DTO dto)
        {
            byte status_online = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            byte Status = 0;
            if (status_offline == 1)
                Status = 0;
            if (status_hidden == 1)
                Status = 1;
            if (status_online == 1)
                Status = 2;
            if (status_away == 1)
                Status = 3;
            if (status_dnd == 1)
                Status = 4;
            if (status_custom == 1)
                Status = 5;
            return Status;
        }
        public ActionResult<bool> Update_Chat_Web_Socket_Log_Tbl(Update_ChatWebSocketLogTbl dto)
        {
            try
            {
                Chat_WebSocketLogTbl.Where(x => x.User_id == dto.Sent_from && x.Sent_to == dto.Sent_to).ExecuteUpdateAsync(s => s
                    .SetProperty(dto => dto.Requested, dto.Requested)
                    .SetProperty(dto => dto.Blocked, dto.Blocked)
                    .SetProperty(dto => dto.Approved, dto.Approved)
                    .SetProperty(dto => dto.Updated_on, TimeStamp)
                    .SetProperty(dto => dto.Updated_by, dto.ID)
                );
                SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }        
        public ActionResult<string> Update_Unconfirmed_Email(DTO dto)
        {
            Unconfirmed_EmailAddressTbl.Where(x => x.Email == dto.Email_address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Code, dto.Code)
                .SetProperty(col => col.Language_Region, dto.Language)
                .SetProperty(col => col.Updated_on, dto.Updated_on)
                .SetProperty(col => col.Updated_by, 0F)
                .SetProperty(col => col.Created_on, TimeStamp)
            );
            SaveChangesAsync();
            obj.email_address = dto.Email_address;
            obj.updated_on = TimeStamp;
            obj.language = dto.Language;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Update_Unconfirmed_Phone(DTO dto)
        {
            Unconfirmed_TelephoneTbl.Where(x => x.Phone == dto.Phone).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Country, dto.Country)
                .SetProperty(col => col.Carrier, dto.Carrier)
                .SetProperty(col => col.Code, dto.Code)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, 0F)
                .SetProperty(col => col.Created_on, TimeStamp));
            SaveChangesAsync();
            obj.Updated_on = TimeStamp;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Update_User_Avatar(DTO dto)
        {
            Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Avatar_title, dto.Avatar_title)
                .SetProperty(col => col.Avatar_url_path, dto.Avatar_url_path)
            );
            SaveChangesAsync();
            return Read_User(dto);
        }
        public ActionResult<string> Update_User_Display_Name(DTO dto)
        {
            Selected_NameTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Name, dto.Display_name)
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            SaveChangesAsync();
            return Read_User(dto);
        }
        public ActionResult<string> Update_User_Login(DTO dto)
        {
            Login_TSTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Login_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            SaveChangesAsync();

            //Getting Authenticated User's Information from Database (if this isn't the first time login which results empty string).
            string? Email = Login_EmailAddressTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Email).SingleOrDefault();
            string? RegionCode = Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? DisplayName = Selected_NameTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Name).SingleOrDefault();
            ulong Created_on = User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();
            byte status_online = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            byte Status = 0;

            if (status_offline == 1)
                Status = 0;
            if (status_hidden == 1)
                Status = 1;
            if (status_online == 1)
                Status = 2;
            if (status_away == 1)
                Status = 3;
            if (status_dnd == 1)
                Status = 4;
            if (status_custom == 1)
                Status = 5;

            obj.id = dto.ID;
            obj.email_address = Email;
            obj.display_name = DisplayName;
            obj.token = Create_Jwt_Token(@$"{dto.ID}");
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.language = @$"{LanguageCode}-{RegionCode}";
            obj.online_status = Status;
            obj.nav_lock = dto.Nav_lock;
            obj.theme = dto.Theme;
            obj.alignment = dto.Alignment;
            obj.text_alignment = dto.Text_alignment;
            obj.login_on = TimeStamp;

            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<bool> Update_User_Logut(ulong id)
        {
            if (ID_Exists_In_Logout_Tbl(id)) {
                Logout_TSTbl.Where(x => x.User_ID == id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Logout_on, TimeStamp)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, id)
                );
                SaveChangesAsync();
            } else {
                Logout_TSTbl.AddAsync(new Logout_TSTbl
                {
                    User_ID = id,
                    Logout_on = TimeStamp,
                    Updated_by = id,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp
                });
                SaveChangesAsync();
            }
            return true;
        }
        public ActionResult<bool> Update_User_Password(DTO dto)
        {
            Login_PasswordTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Password, UsersDbC.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Password}MPCSalt")))
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            SaveChangesAsync();
            return true;
        }
        public ActionResult<string> Update_User_Selected_Alignment(DTO dto)
        {
            switch (dto.Alignment) {
                case 0:
                    Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 1)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 2:
                    Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 1)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 1:
                    Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 1)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                default:
                    return Read_User(dto);
            }
        }
        public ActionResult<string> Update_User_Selected_TextAlignment(DTO dto)
        {
            switch (dto.Text_alignment)
            {
                case 0:
                    Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 1)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 2:
                    Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 1)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 1:
                    Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 1)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                default:
                    return Read_User(dto);
            }
        }
        public ActionResult<string> Update_User_Selected_Language(Selected_LanguageTbl dto)
        {
            Selected_LanguageTbl.Where(x => x.User_ID == dto.User_ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Language_code, dto.Language_code)
                .SetProperty(col => col.Region_code, dto.Region_code)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, dto.Updated_by)
            );
            SaveChangesAsync();
            return @$"{dto.Language_code}-{dto.Region_code}";
        }
        public ActionResult<string> Update_User_Selected_Nav_Lock(DTO dto)
        {
            Selected_NavbarLockTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Locked, dto.Nav_lock)
            );
            SaveChangesAsync();
            return Read_User(dto);
        }
        public ActionResult<string> Update_User_Selected_Status(DTO dto)
        {
            switch (dto.Online_status)
            {
                case 0:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 1)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 1:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 0)
                        .SetProperty(col => col.Hidden, 1)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 2:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 0)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 1)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);                
                case 3:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 0)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 1)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 4:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 0)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 1)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 5:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 0)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 1)
                        .SetProperty(col => col.Custom_lbl, dto.Custom_lbl)
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 10:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 1)
                        .SetProperty(col => col.Hidden, 1)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 20:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 1)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 1)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 30:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 1)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 1)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 40:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 1)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 1)
                        .SetProperty(col => col.Custom, 0)
                        .SetProperty(col => col.Custom_lbl, "")
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 50:
                    Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Offline, 1)
                        .SetProperty(col => col.Hidden, 0)
                        .SetProperty(col => col.Online, 0)
                        .SetProperty(col => col.Away, 0)
                        .SetProperty(col => col.DND, 0)
                        .SetProperty(col => col.Custom, 1)
                        .SetProperty(col => col.Custom_lbl, dto.Custom_lbl)
                        .SetProperty(col => col.Updated_by, dto.ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                default:
                    return Read_User(dto);
            }
        }
        public ActionResult<string> Update_User_Selected_Theme(DTO dto)
        {
            switch (dto.Theme) {
                case 0:
                    Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Light, 1)
                        .SetProperty(col => col.Night, 0)
                        .SetProperty(col => col.Custom, 0)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 1:
                    Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Light, 0)
                        .SetProperty(col => col.Night, 1)
                        .SetProperty(col => col.Custom, 0)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                case 2:
                    Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Light, 0)
                        .SetProperty(col => col.Night, 0)
                        .SetProperty(col => col.Custom, 1)
                    );
                    SaveChangesAsync();
                    return Read_User(dto);
                default:
                    return Read_User(dto);
            }
        }
        public ActionResult<string> Read_Users()
        {
            obj.logoutsTS = Logout_TSTbl.Select(x => x).ToList();
            obj.loginsTS = Login_TSTbl.Select(x => x).ToList();
            obj.display_names = Selected_NameTbl.Select(x => x).ToList();
            obj.avatars = Selected_AvatarTbl.Select(x => x).ToList();
            obj.languages = Selected_LanguageTbl.Select(x => x).ToList();
            return JsonSerializer.Serialize(obj);
        }
        public bool Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(string email)
        {
            return Unconfirmed_EmailAddressTbl.Any(e => e.Email == email);
        }
        public bool Confirmation_Code_Exists_In_Not_Confirmed_Email_Address_Tbl(string Code)
        {
            return Unconfirmed_EmailAddressTbl.Any(e => e.Code == Code);
        }
        public bool Phone_Exists_In_Login_Telephone_Tbl(string phone)
        {
            return Login_TelephoneTbl.Any(e => e.Phone == phone);
        }
        public bool Phone_Exists_In_Telephone_Not_Confirmed_Tbl(string phone)
        {
            return Unconfirmed_TelephoneTbl.Any(e => e.Phone == phone);
        }
        public bool ID_Exist_In_Users_Tbl(ulong id)
        {
            return User_IDsTbl.Any(e => e.ID == id);
        }
        public bool ID_Exists_In_Logout_Tbl(ulong id)
        {
            return Logout_TSTbl.Any(e => e.ID == id);
        }
        public ulong Get_User_ID_By_Email_Address(string email_address)
        {
            return Login_EmailAddressTbl.Where(e => e.Email == email_address).Select(e => e.User_ID).SingleOrDefault();
        }
        public string? Get_User_Email_By_ID(ulong id)
        {
            return Login_EmailAddressTbl.Where(e => e.User_ID == id).Select(e => e.Email).SingleOrDefault();
        }
        public ulong Get_User_ID_From_JWToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
            List<object> values = jwtSecurityToken.Payload.Values.ToList();
            ulong currentTime = Convert.ToUInt64(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            ulong token_expire = Convert.ToUInt64(values[2]);
            bool tokenExpired = token_expire < currentTime ? true : false;

            if (tokenExpired)
                return 0;

            return Convert.ToUInt64(values[0].ToString());
        }
        public static byte[] Create_Salted_Hash_String(byte[] plainText, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] plainTextWithSaltBytes = new byte[plainText.Length + salt.Length];

            for (int i = 0; i < plainText.Length; i++)
            {
                plainTextWithSaltBytes[i] = plainText[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                plainTextWithSaltBytes[plainText.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(plainTextWithSaltBytes);
        }
        public static bool Compare_Password_Byte_Arrays(byte[] array1, byte[] array2)
        {
            if (array1.Length != array2.Length)
            {
                return false;
            }

            for (int i = 0; i < array1.Length; i++)
            {
                if (array1[i] != array2[i])
                {
                    return false;
                }
            }

            return true;
        }
        public byte[]? Get_User_Password_Hash_By_ID(ulong user_id)
        {
            return Login_PasswordTbl.Where(user => user.User_ID == user_id).Select(user => user.Password).SingleOrDefault();
        }
        private void Create_EndUser_Database_Fields(dynamic dto)
        {
            Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                ID = Convert.ToUInt64(Selected_NameTbl.Count() + 1),
                User_ID = dto.id,
                Name = $"Recruit#{dto.id}",
                Created_on = TimeStamp,
                Updated_by = 0,
                Updated_on = TimeStamp
            });

            Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                ID = Convert.ToUInt64(Selected_LanguageTbl.Count() + 1),
                User_ID = dto.id,
                Language_code = dto.language.Substring(0, 2),
                Region_code = dto.language.Substring(3, 2),
                Updated_by = dto.id,
                Updated_on = TimeStamp,
                Created_on = TimeStamp
            });

            Selected_StatusTbl.AddAsync(new Selected_StatusTbl
            {
                ID = Convert.ToUInt64(Selected_StatusTbl.Count() + 1),
                User_ID = dto.id,
                Online = 1,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0,
            });

            Selected_AvatarTbl.AddAsync(new Selected_AvatarTbl
            {
                ID = Convert.ToUInt64(Selected_AvatarTbl.Count() + 1),
                User_ID = dto.id,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0,
            });

            Selected_NavbarLockTbl.AddAsync(new Selected_NavbarLockTbl
            {
                ID = Convert.ToUInt64(Selected_NavbarLockTbl.Count() + 1),
                User_ID = dto.id,
                Locked = dto.nav_lock,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0,
            });

            if (dto.theme == 0) {
                Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                {
                    ID = Convert.ToUInt64(Selected_ThemeTbl.Count() + 1),
                    User_ID = dto.id,
                    Light = 1,
                    Night = 0,
                    Custom = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            } else if (dto.theme == 1) {
                Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                {
                    ID = Convert.ToUInt64(Selected_ThemeTbl.Count() + 1),
                    User_ID = dto.id,
                    Light = 0,
                    Night = 1,
                    Custom = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            } else if (dto.theme == 2) {
                Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                {
                    ID = Convert.ToUInt64(Selected_ThemeTbl.Count() + 1),
                    User_ID = dto.id,
                    Light = 0,
                    Night = 0,
                    Custom = 1,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }

            if (dto.alignment == 0) {
                Selected_AlignmentTbl.AddAsync(new Selected_AlignmentTbl
                {
                    ID = Convert.ToUInt64(Selected_AlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 1,
                    Center = 0,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            } else if (dto.alignment == 1) {
                Selected_AlignmentTbl.AddAsync(new Selected_AlignmentTbl
                {
                    ID = Convert.ToUInt64(Selected_AlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 1,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            } else if (dto.alignment == 2) {
                Selected_AlignmentTbl.AddAsync(new Selected_AlignmentTbl
                {
                    ID = Convert.ToUInt64(Selected_AlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 0,
                    Right = 1,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }

            if (dto.text_alignment == 0) {
                Selected_AppTextAlignmentTbl.AddAsync(new Selected_AppTextAlignmentTbl
                {
                    ID = Convert.ToUInt64(Selected_AppTextAlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 1,
                    Center = 0,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            } else if (dto.text_alignment == 1) {
                Selected_AppTextAlignmentTbl.AddAsync(new Selected_AppTextAlignmentTbl
                {
                    ID = Convert.ToUInt64(Selected_AppTextAlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 1,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            } else if (dto.text_alignment == 2) {
                Selected_AppTextAlignmentTbl.AddAsync(new Selected_AppTextAlignmentTbl
                {
                    ID = Convert.ToUInt64(Selected_AppTextAlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 0,
                    Right = 1,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            SaveChangesAsync();
        }
        public bool Email_Exists_In_Login_EmailAddress_Tbl(string email)
        {
            return Login_EmailAddressTbl.Any(x => x.Email == email);
        }
        private string Create_Jwt_Token(string id)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $@"{id}"),
                new Claim(ClaimTypes.Role, "MPC-End-User"),
            };
            #pragma warning disable CS8604 // Possible null reference argument.
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            #pragma warning restore CS8604 // Possible null reference argument.
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.UtcNow.AddMinutes(TokenExpireTime),
                signingCredentials: signIn);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public ActionResult<string> Create_Integration_Twitch_Record(DTO dto)
        {

            obj.id = dto.ID;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_First_Name(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) {//Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    First_Name = dto.First_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else {//Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.First_Name, dto.First_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.first_name = dto.First_name;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_Last_Name(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) {//Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Last_Name = dto.Last_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else {//Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Last_Name, dto.Last_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.last_name = dto.Last_name;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_Middle_Name(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) { //Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Middle_Name = dto.Middle_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else { //Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Middle_Name, dto.Middle_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.middle_name = dto.Middle_name;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_Maiden_Name(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) { //Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Maiden_Name = dto.Maiden_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else { //Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Maiden_Name, dto.Maiden_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.maiden_name = dto.Maiden_name;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_Gender(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) {//Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Gender = dto.Gender,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else { //Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Gender, dto.Gender)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.gender = dto.Gender;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_Ethnicity(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) { //Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Ethnicity = dto.Ethnicity,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else { //Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Ethnicity, dto.Ethnicity)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.ethnicity = dto.Ethnicity;
            return JsonSerializer.Serialize(obj);
        }
        public ActionResult<string> Save_End_User_Birth_Date(DTO dto)
        {
            if (!IdentityTbl.Any(x => x.User_ID == dto.ID)) { //Insert
                IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    B_Month = dto.Month,
                    B_Day = dto.Day,
                    B_Year = dto.Year,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            } else { //Update
                IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.B_Month, dto.Month)
                    .SetProperty(col => col.B_Day, dto.Day)
                    .SetProperty(col => col.B_Year, dto.Year)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            SaveChangesAsync();
            obj.id = dto.ID;
            obj.birth_month = dto.Month;
            obj.birth_day = dto.Day;
            obj.birth_year = dto.Year;
            return JsonSerializer.Serialize(obj);
        }
    }//DbContext
    public class DTO
    {
        public ulong ID { get; set; }
        public string Token { get; set; } = string.Empty;
        public string? First_name { get; set; } = string.Empty;
        public string? Middle_name { get; set; } = string.Empty;
        public string? Last_name { get; set; } = string.Empty;
        public string? Maiden_name { get; set; } = string.Empty;
        public byte Gender { get; set; }
        public byte Month { get; set; }
        public byte Day { get; set; }
        public ulong Year { get; set; }
        public string? Ethnicity { get; set; } = string.Empty;
        public bool? Nav_lock { get; set; }
        public byte? Country { get; set; }
        public byte? Online_status { get; set; }
        public byte? Theme { get; set; }
        public byte? Alignment { get; set; }
        public byte? Text_alignment { get; set; }
        public ulong Target_id { get; set; }
        public ulong? Created_on { get; set; }
        public ulong? Updated_on { get; set; }
        public ulong? Login_on { get; set; }
        public ulong? Logout_on { get; set; }
        public string? Target_name { get; set; } = string.Empty;
        public string? Carrier { get; set; } = string.Empty;
        public string? Email_address { get; set; } = string.Empty;
        public string? Password { get; set; } = string.Empty;
        public string? New_password { get; set; } = string.Empty;
        public string? Phone { get; set; } = string.Empty;
        public string? Language { get; set; } = string.Empty;
        public string? Avatar_title { get; set; } = string.Empty;
        public string? Avatar_url_path { get; set; } = string.Empty;
        public string? Name { get; set; } = string.Empty;
        public string? Display_name { get; set; } = string.Empty;
        public string? Custom_lbl { get; set; } = string.Empty;
        public string? Code { get; set; } = string.Empty;
        public DateTime Token_expire { get; set; }
    }
}//NameSpace