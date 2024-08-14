using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Confirmation;
using mpc_dotnetc_user_server.Models.Users.Selections;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Chat;
using System.Dynamic;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Authentication;
using Microsoft.Extensions.Configuration;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class UsersRepository : IUsersRepository
    {
        private readonly int TokenExpireTime = 9999;//JWT expire in Minutes.
        private readonly ulong TimeStamp = Convert.ToUInt64(DateTimeOffset.Now.ToUnixTimeSeconds());
        private dynamic obj = new ExpandoObject();
        private readonly IConfiguration _configuration;
        private readonly UsersDBC _UsersDBC;

        public UsersRepository(IConfiguration configuration, UsersDBC UsersDBC)
        {
            _UsersDBC = UsersDBC;
            _configuration = configuration;
        }
        public async Task<string> Create_Unconfirmed_Email(DTO dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.AddAsync(new Pending_Email_RegistrationTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Pending_Email_RegistrationTbl.Count() + 1),
                Email = dto.Email_Address,
                Code = dto.Code,
                Language_Region = dto.Language,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });

            await _UsersDBC.SaveChangesAsync();
            obj.email_address = dto.Email_Address;
            obj.code = dto.Code;
            obj.language = dto.Language;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<bool> Email_Exists_In_Login_Email_Address_Tbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_EmailAddressTbl.Any(x => x.Email == email_address));
        }
        public async Task<string> Create_Unconfirmed_Phone(DTO dto)
        {
            await _UsersDBC.Unconfirmed_TelephoneTbl.AddAsync(new Unconfirmed_TelephoneTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Unconfirmed_TelephoneTbl.Count() + 1),
                Country = dto.Country,
                Phone = dto.Phone,
                Code = dto.Code,
                Carrier = dto.Carrier,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });

            await _UsersDBC.SaveChangesAsync();

            obj.phone = dto.Phone;
            obj.country = dto.Country;
            obj.carrier = dto.Carrier;
            obj.code = dto.Code;

            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Create_Account_By_Email(DTO dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, 0F)
                .SetProperty(col => col.Deleted_by, 0F)
            );
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email = dto.Email_Address,
                Updated_on = TimeStamp,
                Updated_by = 0,
                Language_Region = dto.Language,
                Created_on = TimeStamp,
            });
            await _UsersDBC.SaveChangesAsync();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.User_IDsTbl.Count() + 1),
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            };
            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_EmailAddressTbl.AddAsync(new Login_EmailAddressTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_EmailAddressTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Email = dto.Email_Address,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_PasswordTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Password = Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}MPCSalt")).Result,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_TSTbl.AddAsync(new Login_TSTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Login_on = TimeStamp,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Logout_TSTbl.AddAsync(new Logout_TSTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Logout_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Logout_on = 0,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            await _UsersDBC.SaveChangesAsync();

            obj.id = ID_Record.ID;
            obj.email_address = dto.Email_Address;
            obj.token = Create_Jwt_Token(@$"{ID_Record.ID}");
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.created_on = TimeStamp;
            obj.language = dto.Language;
            obj.alignment = dto.Alignment;
            obj.text_alignment = dto.Text_alignment;
            obj.nav_lock = dto.Nav_lock;
            obj.theme = dto.Theme;
            obj.online_status = 1;
            obj.name = dto.Email_Address;
            obj.display_name = dto.Email_Address;

            Create_End_User_Database_Fields(obj);
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Create_Account_By_Phone(DTO dto)
        {
            await _UsersDBC.Unconfirmed_TelephoneTbl.Where(x => x.Phone == dto.Phone).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );

            await _UsersDBC.Confirmed_TelephoneTbl.AddAsync(new Confirmed_TelephoneTbl
            {
                Country = dto.Country,
                Phone = dto.Phone,
                Code = dto.Code,
                Carrier = dto.Carrier,
                Updated_on = TimeStamp
            });
            await _UsersDBC.SaveChangesAsync();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.User_IDsTbl.Count() + 1),
                Created_on = TimeStamp
            };
            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_TelephoneTbl.AddAsync(new Login_TelephoneTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_TelephoneTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Phone = dto.Phone,
                Country = dto.Country,
                Carrier = dto.Carrier,
                Created_on = TimeStamp,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_PasswordTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Password = Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Phone}MPCSalt")).Result,
                Created_on = TimeStamp
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_TSTbl.AddAsync(new Login_TSTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Login_on = TimeStamp,
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Logout_TSTbl.AddAsync(new Logout_TSTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Logout_TSTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Logout_on = 0,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
            });
            await _UsersDBC.SaveChangesAsync();

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

            Create_End_User_Database_Fields(obj);
            return JsonSerializer.Serialize(obj);
        }
        public async Task<bool> Create_Contact_Us_Record(DTO obj)
        {
            await _UsersDBC.Contact_UsTbl.AddAsync(new Contact_UsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Contact_UsTbl.Count() + 1),
                USER_ID = obj.ID,
                Subject_Line = obj.Subject_line,
                Summary = obj.Summary,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Website_Bug_Record(DTO obj)
        {
            await _UsersDBC.Reported_WebsiteBugTbl.AddAsync(new Reported_WebsiteBugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_WebsiteBugTbl.Count() + 1),
                USER_ID = obj.ID,
                URL = obj.URL,
                Detail = obj.Detail,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_WebSocket_Log_Record(DTO obj)
        {
            await _UsersDBC.Websocket_Chat_PermissionTbl.AddAsync(new Websocket_Chat_PermissionTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Websocket_Chat_PermissionTbl.Count() + 1),
                User_id = obj.ID,
                Sent_to = obj.Send_to,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0,
                Requested = 1,
                Approved = 0,
                Blocked = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Discord_Bot_Bug_Record(DTO obj)
        {
            await _UsersDBC.Reported_DiscordBotBugTbl.AddAsync(new Reported_DiscordBotBugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_DiscordBotBugTbl.Count() + 1),
                USER_ID = obj.ID,
                Location = obj.Location,
                Detail = obj.Detail,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Comment_Box_Record(DTO obj)
        {
            await _UsersDBC.Comment_BoxTbl.AddAsync(new Comment_BoxTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Comment_BoxTbl.Count() + 1),
                USER_ID = obj.ID,
                Comment = obj.Comment,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Broken_Link_Record(DTO obj)
        {
            await _UsersDBC.Reported_BrokenLinkTbl.AddAsync(new Reported_BrokenLinkTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_BrokenLinkTbl.Count() + 1),
                USER_ID = obj.ID,
                URL = obj.URL,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_Reported_User_Profile_Record(DTO dto)
        {
            Reported_ProfileTbl record = new Reported_ProfileTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_ProfileTbl.Count() + 1),
                USER_ID = dto.ID,
                Reported_ID = dto.Reported_ID,
                Page_Title = _UsersDBC.ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Page_Title).SingleOrDefault(),
                Page_Description = _UsersDBC.ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Page_Description).SingleOrDefault(),
                About_Me = _UsersDBC.ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.About_Me).SingleOrDefault(),
                Banner_URL = _UsersDBC.ProfilePageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Banner_URL).SingleOrDefault(),
                Reported_Reason = dto.Reported_Reason,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.ID
            };
            await _UsersDBC.Reported_ProfileTbl.AddAsync(record);
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.report_record_id = record.ID;
            obj.reported_user_id = record.Reported_ID;
            obj.created_on = record.Created_on;
            obj.read_reported_user = Read_User(new DTO { ID = dto.Reported_ID }).ToString();
            obj.read_reported_profile = Read_User_Profile(new DTO { ID = dto.Reported_ID }).ToString();
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Delete_Account_By_User_ID(DTO dto)
        {
            await _UsersDBC.User_IDsTbl.Where(x => x.ID == dto.Target_ID).ExecuteUpdateAsync(s => s
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted, 1)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_by, dto.ID)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Created_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_by, dto.ID)
            );
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.Target_id = dto.Target_ID;
            return JsonSerializer.Serialize(obj);
        }
        public Task<string> Read_User(DTO dto)
        {//Getting Information About the End User...
            bool Nav_lock = _UsersDBC.Selected_NavbarLockTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Locked).SingleOrDefault();
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            byte Status = 0;
            byte Light = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Light).SingleOrDefault();
            byte Night = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Night).SingleOrDefault();
            byte CustomTheme = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            byte Theme = 0;
            byte LeftAligned = _UsersDBC.Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Left).SingleOrDefault();
            byte CenterAligned = _UsersDBC.Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Center).SingleOrDefault();
            byte RightAligned = _UsersDBC.Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Right).SingleOrDefault();
            byte Alignment = 0;
            byte LeftTextAligned = _UsersDBC.Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Left).SingleOrDefault();
            byte CenterTextAligned = _UsersDBC.Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Center).SingleOrDefault();
            byte RightTextAligned = _UsersDBC.Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Right).SingleOrDefault();
            byte TextAlignment = 0;
            ulong LoginTS = _UsersDBC.Login_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = _UsersDBC.Logout_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Logout_on).SingleOrDefault();
            ulong Created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();
            string? customLbl = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom_lbl).SingleOrDefault();
            string? Email = _UsersDBC.Login_EmailAddressTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Email).SingleOrDefault();
            string? RegionCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? Avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? Avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_title).SingleOrDefault();
            string? DisplayName = _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Name).SingleOrDefault();
            byte? Gender = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Gender).SingleOrDefault();
            byte? BirthDay = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.B_Day).SingleOrDefault();
            byte? BirthMonth = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.B_Month).SingleOrDefault();
            ulong? BirthYear = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.B_Year).SingleOrDefault();
            string? FirstName = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.First_Name).SingleOrDefault();
            string? LastName = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Last_Name).SingleOrDefault();
            string? MiddleName = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Middle_Name).SingleOrDefault();
            string? MaidenName = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Maiden_Name).SingleOrDefault();
            string? Ethnicity = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Ethnicity).SingleOrDefault();

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

            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public async Task<string> Read_User_Profile(DTO dto)
        {
            //Get Information About the End User for the client side.
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
            string? customLbl = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom_lbl).SingleOrDefault();

            //Send Information to Client Side going below...
            byte Status = 0;

            ulong LoginTS = _UsersDBC.Login_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = _UsersDBC.Logout_TSTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Logout_on).SingleOrDefault();
            ulong Created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();

            string? Email = _UsersDBC.Login_EmailAddressTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Email).SingleOrDefault();
            string? RegionCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? Avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? Avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Avatar_title).SingleOrDefault();
            string? DisplayName = _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Name).SingleOrDefault();

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

            return await Task.FromResult(JsonSerializer.Serialize(new DTO
            {
                ID = dto.ID,
                Email_Address = Email,
                Display_name = DisplayName,
                Login_on = LoginTS,
                Logout_on = LogoutTS,
                Language = @$"{LanguageCode}-{RegionCode}",
                Online_status = Status,
                Custom_lbl = customLbl,
                Created_on = Created_on,
                Avatar_url_path = Avatar_url_path,
                Avatar_title = Avatar_title
            }));
        }
        public Task<string> Read_WebSocket_Permission_Record(DTO dto)
        {
            byte requested = _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_id == dto.ID && x.Sent_to == dto.Send_to).Select(x => x.Requested).SingleOrDefault();
            byte approved = _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_id == dto.ID && x.Sent_to == dto.Send_to).Select(x => x.Approved).SingleOrDefault();
            byte blocked = _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_id == dto.ID && x.Sent_to == dto.Send_to).Select(x => x.Blocked).SingleOrDefault();

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

            if (blocked == 1)
            {
                obj.requested = 0;
                obj.blocked = 1;
                obj.approved = 0;
            }

            if (requested == 0 && approved == 0 && blocked == 0)
            {//When no records are found meaning a request is made from person A to person B.
                Create_Chat_WebSocket_Log_Records(dto);
                obj.requested = 1;
                obj.blocked = 0;
                obj.approved = 0;
            }

            obj.id = dto.ID;
            obj.send_to = dto.Send_to;
            obj.message = dto.Message;

            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public async Task<string> Read_End_User_Web_Socket_Data(DTO dto)
        {
            return await Task.FromResult(JsonSerializer.Serialize(_UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_id == dto.ID).ToList().Concat(_UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.Sent_to == dto.ID).ToList())));
        }
        public async Task<byte> Read_End_User_Selected_Status(DTO dto)
        {
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
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
            return await Task.FromResult(Status);
        }

        public async Task<bool> Create_Reported_WebSocket_Record(DTO dto) 
        {
            try
            {
                await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                {
                    USER_ID = dto.ID,
                    Updated_on = TimeStamp,
                    Updated_by = dto.ID,
                    User = dto.Sent_from,
                    Spam = dto.Spam,
                    Abuse = dto.Abuse,
                    Reason = dto.Reported_Reason,
                    Created_on = TimeStamp,
                });
                await _UsersDBC.SaveChangesAsync();
                return true;
            } catch {
                return false;
            }
        }

        public async Task<bool> Update_Chat_Web_Socket_Permissions_Tbl(DTO dto)
        {
            try
            {
                await _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_id == dto.Sent_from && x.Sent_to == dto.Sent_to).ExecuteUpdateAsync(s => s
                    .SetProperty(dto => dto.Requested, dto.Requested)
                    .SetProperty(dto => dto.Blocked, dto.Blocked)
                    .SetProperty(dto => dto.Approved, dto.Approved)
                    .SetProperty(dto => dto.Updated_on, TimeStamp)
                    .SetProperty(dto => dto.Updated_by, dto.ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
        public async Task<string> Update_Unconfirmed_Email(DTO dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Code, dto.Code)
                .SetProperty(col => col.Language_Region, dto.Language)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            await _UsersDBC.SaveChangesAsync();
            obj.email_address = dto.Email_Address;
            obj.updated_on = TimeStamp;
            obj.language = dto.Language;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_Unconfirmed_Phone(DTO dto)
        {
            await _UsersDBC.Unconfirmed_TelephoneTbl.Where(x => x.Phone == dto.Phone).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Country, dto.Country)
                .SetProperty(col => col.Carrier, dto.Carrier)
                .SetProperty(col => col.Code, dto.Code)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, 0F)
                .SetProperty(col => col.Created_on, TimeStamp));
            await _UsersDBC.SaveChangesAsync();
            obj.Updated_on = TimeStamp;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_User_Avatar(DTO dto)
        {
            await _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Avatar_title, dto.Avatar_title)
                .SetProperty(col => col.Avatar_url_path, dto.Avatar_url_path)
            );
            await _UsersDBC.SaveChangesAsync();
            return await Read_User(dto);
        }
        public async Task<string> Update_User_Display_Name(DTO dto)
        {
            await _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Name, dto.Display_name)
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            await _UsersDBC.SaveChangesAsync();
            return await Read_User(dto);
        }
        public async Task<string> Update_User_Login(DTO dto)
        {
            await _UsersDBC.Login_TSTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Login_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            await _UsersDBC.SaveChangesAsync();

            //Getting Authenticated User's Information from Database (if this isn't the first time login which results empty string).
            string? Email = _UsersDBC.Login_EmailAddressTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Email).SingleOrDefault();
            string? RegionCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? DisplayName = _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Name).SingleOrDefault();
            ulong Created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte? Gender = _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Gender).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).Select(x => x.Custom).SingleOrDefault();
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
            obj.token = Create_Jwt_Token(@$"{dto.ID}").Result;
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.language = @$"{LanguageCode}-{RegionCode}";
            obj.online_status = Status;
            obj.nav_lock = dto.Nav_lock;
            obj.theme = dto.Theme;
            obj.alignment = dto.Alignment;
            obj.text_alignment = dto.Text_alignment;
            obj.login_on = TimeStamp;
            obj.gender = Gender;

            return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
        }
        public async Task<string> Update_User_Logout(ulong id)
        {
            if (ID_Exists_In_Logout_Tbl(id))
            {//update
                await _UsersDBC.Logout_TSTbl.Where(x => x.User_ID == id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Logout_on, TimeStamp)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, id)
                );
                await _UsersDBC.SaveChangesAsync();
            }
            else
            {//insert
                await _UsersDBC.Logout_TSTbl.AddAsync(new Logout_TSTbl
                {
                    User_ID = id,
                    Logout_on = TimeStamp,
                    Updated_by = id,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp
                });
                await _UsersDBC.SaveChangesAsync();
            }
            obj.ID = id;
            obj.logout_on = TimeStamp;
            return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
        }
        public async Task<bool> Update_User_Password(DTO dto)
        {
            await _UsersDBC.Login_PasswordTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Password, Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Password}MPCSalt")).Result)
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Update_User_Selected_Alignment(DTO dto)
        {
            switch (dto.Alignment)
            {
                case 0:
                    await _UsersDBC.Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 1)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 2:
                    await _UsersDBC.Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 1)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 1:
                    await _UsersDBC.Selected_AlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 1)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                default:
                    return await Read_User(dto);
            }
        }
        public async Task<string> Update_User_Selected_TextAlignment(DTO dto)
        {
            switch (dto.Text_alignment)
            {
                case 0:
                    await _UsersDBC.Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 1)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 2:
                    await _UsersDBC.Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 0)
                        .SetProperty(col => col.Right, 1)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 1:
                    await _UsersDBC.Selected_AppTextAlignmentTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Left, 0)
                        .SetProperty(col => col.Center, 1)
                        .SetProperty(col => col.Right, 0)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.ID)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                default:
                    return await Read_User(dto);
            }
        }
        public async Task<string> Update_User_Selected_Language(DTO dto)
        {
            await _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.User_ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Language_code, dto.Language_code)
                .SetProperty(col => col.Region_code, dto.Region_code)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, dto.Updated_by)
            );
            await _UsersDBC.SaveChangesAsync();
            return @$"{dto.Language_code}-{dto.Region_code}";
        }
        public async Task<string> Update_User_Selected_Nav_Lock(DTO dto)
        {
            await _UsersDBC.Selected_NavbarLockTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Updated_by, dto.ID)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Locked, dto.Nav_lock)
            );
            await _UsersDBC.SaveChangesAsync();
            return await Read_User(dto);
        }
        public async Task<string> Update_User_Selected_Status(DTO dto)
        {
            switch (dto.Online_status)
            {
                case 0:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 1:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 2:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 3:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 4:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 5:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 10:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 20:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 30:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 40:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 50:
                    await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
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
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                default:
                    return await Read_User(dto);
            }
        }
        public async Task<string> Update_User_Selected_Theme(DTO dto)
        {
            switch (dto.Theme)
            {
                case 0:
                    await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Light, 1)
                        .SetProperty(col => col.Night, 0)
                        .SetProperty(col => col.Custom, 0)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 1:
                    await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Light, 0)
                        .SetProperty(col => col.Night, 1)
                        .SetProperty(col => col.Custom, 0)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                case 2:
                    await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Light, 0)
                        .SetProperty(col => col.Night, 0)
                        .SetProperty(col => col.Custom, 1)
                    );
                    await _UsersDBC.SaveChangesAsync();
                    return await Read_User(dto);
                default:
                    return await Read_User(dto);
            }
        }
        public Task<string> Read_Users()
        {
            obj.logoutsTS = _UsersDBC.Logout_TSTbl.Select(x => x).ToList();
            obj.loginsTS = _UsersDBC.Login_TSTbl.Select(x => x).ToList();
            obj.display_names = _UsersDBC.Selected_NameTbl.Select(x => x).ToList();
            obj.avatars = _UsersDBC.Selected_AvatarTbl.Select(x => x).ToList();
            obj.languages = _UsersDBC.Selected_LanguageTbl.Select(x => x).ToList();
            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public async Task<bool> Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(e => e.Email == email_address));
        }
        public async Task<bool> Confirmation_Code_Exists_In_Not_Confirmed_Email_Address_Tbl(string Code)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(e => e.Code == Code));
        }
        public async Task<bool> Telephone_Exists_In_Login_Telephone_Tbl(string telephone_number)
        {
            return await Task.FromResult(_UsersDBC.Login_TelephoneTbl.Any(e => e.Phone == telephone_number));
        }
        public async Task<bool> Phone_Exists_In_Telephone_Not_Confirmed_Tbl(string telephone_number)
        {
            return await Task.FromResult(_UsersDBC.Unconfirmed_TelephoneTbl.Any(e => e.Phone == telephone_number));
        }
        public async Task<bool> ID_Exists_In_Users_Tbl(ulong id)
        {
            return await Task.FromResult(_UsersDBC.User_IDsTbl.Any(e => e.ID == id));
        }
        public bool ID_Exists_In_Logout_Tbl(ulong id)
        {
            return _UsersDBC.Logout_TSTbl.Any(e => e.ID == id);
        }
        public async Task<ulong> Get_User_ID_By_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_EmailAddressTbl.Where(e => e.Email == email_address).Select(e => e.User_ID).SingleOrDefault());
        }
        public async Task<string?> Get_User_Email_By_ID(ulong id)
        {
            return await Task.FromResult(_UsersDBC.Login_EmailAddressTbl.Where(e => e.User_ID == id).Select(e => e.Email).SingleOrDefault());
        }
        public async Task<ulong> Get_User_ID_From_JWToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
            List<object> values = jwtSecurityToken.Payload.Values.ToList();
            ulong currentTime = Convert.ToUInt64(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            ulong token_expire = Convert.ToUInt64(values[2]);
            bool tokenExpired = token_expire < currentTime ? true : false;

            if (tokenExpired)
                return 0;

            return await Task.FromResult(Convert.ToUInt64(values[0].ToString()));
        }
        public async Task<byte[]> Create_Salted_Hash_String(byte[] text, byte[] salt)
        {
            HashAlgorithm algorithm = new SHA256Managed();

            byte[] textWithSaltBytes = new byte[text.Length + salt.Length];

            for (int i = 0; i < text.Length; i++)
            {
                textWithSaltBytes[i] = text[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                textWithSaltBytes[text.Length + i] = salt[i];
            }

            return await Task.FromResult(algorithm.ComputeHash(textWithSaltBytes));
        }
        public bool Compare_Password_Byte_Arrays(byte[] array1, byte[] array2)
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
        public async Task<byte[]?> Get_User_Password_Hash_By_ID(ulong user_id)
        {
            return await Task.FromResult(_UsersDBC.Login_PasswordTbl.Where(user => user.User_ID == user_id).Select(user => user.Password).SingleOrDefault());
        }
        public async void Create_End_User_Database_Fields(dynamic dto)
        {
            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_NameTbl.Count() + 1),
                User_ID = dto.id,
                Name = $"Recruit#{dto.id}",
                Created_on = TimeStamp,
                Updated_by = 0,
                Updated_on = TimeStamp
            });

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_LanguageTbl.Count() + 1),
                User_ID = dto.id,
                Language_code = dto.language.Substring(0, 2),
                Region_code = dto.language.Substring(3, 2),
                Updated_by = dto.id,
                Updated_on = TimeStamp,
                Created_on = TimeStamp
            });

            await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                User_ID = dto.id,
                Online = 1,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0,
            });

            await _UsersDBC.Selected_AvatarTbl.AddAsync(new Selected_AvatarTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_AvatarTbl.Count() + 1),
                User_ID = dto.id,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0,
            });

            await _UsersDBC.Selected_NavbarLockTbl.AddAsync(new Selected_NavbarLockTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_NavbarLockTbl.Count() + 1),
                User_ID = dto.id,
                Locked = dto.nav_lock,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0,
            });

            await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                User_ID = dto.id,
                Gender = 2,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.id
            });

            if (dto.theme == 0)
            {
                await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                    User_ID = dto.id,
                    Light = 1,
                    Night = 0,
                    Custom = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            else if (dto.theme == 1)
            {
                await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                    User_ID = dto.id,
                    Light = 0,
                    Night = 1,
                    Custom = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            else if (dto.theme == 2)
            {
                await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                    User_ID = dto.id,
                    Light = 0,
                    Night = 0,
                    Custom = 1,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }

            if (dto.alignment == 0)
            {
                await _UsersDBC.Selected_AlignmentTbl.AddAsync(new Selected_AlignmentTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_AlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 1,
                    Center = 0,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            else if (dto.alignment == 1)
            {
                await _UsersDBC.Selected_AlignmentTbl.AddAsync(new Selected_AlignmentTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_AlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 1,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            else if (dto.alignment == 2)
            {
                await _UsersDBC.Selected_AlignmentTbl.AddAsync(new Selected_AlignmentTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_AlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 0,
                    Right = 1,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }

            if (dto.text_alignment == 0)
            {
                await _UsersDBC.Selected_AppTextAlignmentTbl.AddAsync(new Selected_AppTextAlignmentTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_AppTextAlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 1,
                    Center = 0,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            else if (dto.text_alignment == 1)
            {
                await _UsersDBC.Selected_AppTextAlignmentTbl.AddAsync(new Selected_AppTextAlignmentTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_AppTextAlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 1,
                    Right = 0,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }
            else if (dto.text_alignment == 2)
            {
                await _UsersDBC.Selected_AppTextAlignmentTbl.AddAsync(new Selected_AppTextAlignmentTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Selected_AppTextAlignmentTbl.Count() + 1),
                    User_ID = dto.id,
                    Left = 0,
                    Center = 0,
                    Right = 1,
                    Created_on = TimeStamp,
                    Updated_on = TimeStamp,
                    Updated_by = 0,
                });
            }

            await _UsersDBC.SaveChangesAsync();
        }
        public async Task<string> Create_Jwt_Token(string id)
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

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }
        public async Task<string> Create_Integration_Twitch_Record(DTO dto)
        {

            obj.id = dto.ID;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Save_End_User_First_Name(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            {//Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    First_Name = dto.First_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            {//Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.First_Name, dto.First_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.first_name = dto.First_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Save_End_User_Last_Name(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            {//Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Last_Name = dto.Last_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            {//Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Last_Name, dto.Last_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.last_name = dto.Last_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Save_End_User_Middle_Name(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Middle_Name = dto.Middle_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                     .SetProperty(col => col.Middle_Name, dto.Middle_name)
                     .SetProperty(col => col.Updated_on, TimeStamp)
                     .SetProperty(col => col.Updated_by, dto.ID)
                 );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.middle_name = dto.Middle_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Save_End_User_Maiden_Name(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Maiden_Name = dto.Maiden_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Maiden_Name, dto.Maiden_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.maiden_name = dto.Maiden_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Gender(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            {//Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Gender = dto.Gender,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            { //Update

                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Gender, dto.Gender)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.gender = dto.Gender;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Save_End_User_Ethnicity(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    Ethnicity = dto.Ethnicity,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Ethnicity, dto.Ethnicity)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.ethnicity = dto.Ethnicity;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Save_End_User_Birth_Date(DTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.ID))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.ID,
                    B_Month = dto.Month,
                    B_Day = dto.Day,
                    B_Year = dto.Year,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.ID
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.B_Month, dto.Month)
                    .SetProperty(col => col.B_Day, dto.Day)
                    .SetProperty(col => col.B_Year, dto.Year)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.ID;
            obj.birth_month = dto.Month;
            obj.birth_day = dto.Day;
            obj.birth_year = dto.Year;
            return JsonSerializer.Serialize(obj);
        }
        public void Create_Chat_WebSocket_Log_Records(DTO dto)
        {
            _UsersDBC.Websocket_Chat_PermissionTbl.AddAsync(new Websocket_Chat_PermissionTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Websocket_Chat_PermissionTbl.Count() + 1),
                User_id = dto.ID,
                Sent_to = dto.Send_to,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.ID,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            _UsersDBC.SaveChangesAsync();
            _UsersDBC.Websocket_Chat_PermissionTbl.AddAsync(new Websocket_Chat_PermissionTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Websocket_Chat_PermissionTbl.Count() + 1),
                User_id = dto.Send_to,//Swapped so we are to create the record for the other user.
                Sent_to = dto.ID,//Ditto.
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.ID,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            _UsersDBC.SaveChangesAsync();
        }
    }
}
