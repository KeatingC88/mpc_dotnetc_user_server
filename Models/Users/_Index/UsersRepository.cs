using System.Dynamic;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Authentication;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.BirthDate;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Controllers.Users.AES;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class UsersRepository : IUsersRepository
    {
        private readonly int TokenExpireTime = 9999;//JWT expired in integer Minutes.
        private readonly ulong TimeStamp = Convert.ToUInt64(DateTimeOffset.Now.ToUnixTimeSeconds());
        private dynamic obj = new ExpandoObject();
        private readonly IConfiguration _configuration;
        private readonly UsersDBC _UsersDBC;

        AES_RW AES_RW = new AES_RW();

        public UsersRepository(IConfiguration configuration, UsersDBC UsersDBC)
        {
            _UsersDBC = UsersDBC;
            _configuration = configuration;
        }

        public async Task<string> Create_Account_By_Email(Complete_Email_RegistrationDTO dto)
        {
            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.User_IDsTbl.Count() + 1),
                Created_by = 0,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0
            };
            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, ID_Record.ID)
                .SetProperty(col => col.Deleted_by, ID_Record.ID)
            );
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address,
                Updated_on = TimeStamp,
                Updated_by = (ulong)0,
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = TimeStamp,
                Created_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_Email_AddressTbl.AddAsync(new Login_Email_AddressTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_Email_AddressTbl.Count() + 1),
                User_id = ID_Record.ID,
                Email_Address = dto.Email_Address,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_PasswordTbl.Count() + 1),
                User_id = ID_Record.ID,
                Password = Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}MPCSalt")).Result,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Language(new Selected_LanguageDTO {
                User_id = ID_Record.ID,
                Language = dto.Language,
                Region = dto.Region
            });

            await Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO
            {
                User_id = ID_Record.ID,
                Alignment = dto.Alignment,
            });
            obj.alignment = dto.Alignment;

            await Update_End_User_Selected_Theme(new Selected_ThemeDTO
            {
                User_id = ID_Record.ID,
                Theme = dto.Theme,
            });
            obj.theme = dto.Theme;
            
            await Update_End_User_Selected_Nav_Lock(new Selected_Navbar_LockDTO
            {
                User_id = ID_Record.ID,
                Locked = dto.Nav_lock,
            });
            obj.nav_lock = dto.Nav_lock;

            await Update_End_User_Selected_TextAlignment(new Selected_App_Text_AlignmentDTO
            {
                User_id = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });
            obj.text_alignment = dto.Text_alignment;

            obj.id = ID_Record.ID;
            obj.email_address = dto.Email_Address;
            obj.token = Create_Jwt_Token(@$"{ID_Record.ID}");
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.created_on = TimeStamp;
            obj.language_region = @$"{dto.Language}-{dto.Region}";
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Create_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.AddAsync(new Pending_Email_RegistrationTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Pending_Email_RegistrationTbl.Count() + 1),
                Email_Address = dto.Email_Address,
                Code = dto.Code,
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_by = 0,
                Created_on = TimeStamp,
                Updated_on = TimeStamp,
                Updated_by = 0
            });

            await _UsersDBC.SaveChangesAsync();
            obj.email_address = AES_RW.Process_Encryption(dto.Email_Address);
            obj.code = AES_RW.Process_Encryption(dto.Code);
            obj.language = AES_RW.Process_Encryption(dto.Language);
            obj.region = AES_RW.Process_Encryption(dto.Region);
            obj.created_on = AES_RW.Process_Encryption(TimeStamp.ToString());
            return JsonSerializer.Serialize(obj);
        }        
        public async Task<bool> Email_Exists_In_Login_Email_AddressTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Any(x => x.Email_Address == email_address));
        }
        public async Task<string> Create_Unconfirmed_Phone(DTO dto)
        {
            await _UsersDBC.Pending_Telephone_RegistrationTbl.AddAsync(new Pending_Telephone_RegistrationTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Pending_Telephone_RegistrationTbl.Count() + 1),
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
        public async Task<string> Create_Account_By_Phone(DTO dto)
        {
            await _UsersDBC.Pending_Telephone_RegistrationTbl.Where(x => x.Phone == dto.Phone).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
            );

            await _UsersDBC.Completed_Telephone_RegistrationTbl.AddAsync(new Completed_Telephone_RegistrationTbl
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
                User_id = ID_Record.ID,
                Phone = dto.Phone,
                Country = dto.Country,
                Carrier = dto.Carrier,
                Created_on = TimeStamp,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            obj.id = ID_Record.ID;
            obj.token = Create_Jwt_Token(@$"{ID_Record.ID}");
            obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
            obj.created_on = TimeStamp;
            obj.phone = dto.Phone;
            obj.country = dto.Country;
            obj.carrier = dto.Carrier;
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
        public async Task<bool> Create_End_User_Status_Record(Selected_StatusDTO obj)
        {
            await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                User_id = obj.User_id,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Created_by = obj.User_id,
                Online = 1,
                Updated_by = obj.User_id
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Website_Bug_Record(DTO obj)
        {
            await _UsersDBC.Reported_Website_BugTbl.AddAsync(new Reported_Website_BugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Website_BugTbl.Count() + 1),
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
        public async Task<string> Create_WebSocket_Log_Record(Websocket_Chat_PermissionDTO dto)
        {
            try { 
                await _UsersDBC.Websocket_Chat_PermissionTbl.AddAsync(new Websocket_Chat_PermissionTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Websocket_Chat_PermissionTbl.Count() + 1),
                    User_A_id = dto.User_id,
                    User_B_id = dto.User_B_id,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Requested = 1,
                    Approved = 0,
                    Blocked = 0
                });
                await _UsersDBC.SaveChangesAsync();
                obj.updated_on = TimeStamp;
                obj.updated_by = dto.User_id;
                obj.updated_for = dto.User_B_id;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: WebSocket Log Record";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<bool> Create_Discord_Bot_Bug_Record(DTO obj)
        {
            await _UsersDBC.Reported_Discord_Bot_BugTbl.AddAsync(new Reported_Discord_Bot_BugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Discord_Bot_BugTbl.Count() + 1),
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
            await _UsersDBC.Reported_Broken_LinkTbl.AddAsync(new Reported_Broken_LinkTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Broken_LinkTbl.Count() + 1),
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
                Page_Title = _UsersDBC.ProfilePageTbl.Where(x => x.User_id == dto.Reported_ID).Select(x => x.Page_Title).SingleOrDefault(),
                Page_Description = _UsersDBC.ProfilePageTbl.Where(x => x.User_id == dto.Reported_ID).Select(x => x.Page_Description).SingleOrDefault(),
                About_Me = _UsersDBC.ProfilePageTbl.Where(x => x.User_id == dto.Reported_ID).Select(x => x.About_Me).SingleOrDefault(),
                Banner_URL = _UsersDBC.ProfilePageTbl.Where(x => x.User_id == dto.Reported_ID).Select(x => x.Banner_URL).SingleOrDefault(),
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
            obj.read_reported_user = Read_User(dto.Reported_ID).ToString();
            obj.read_reported_profile = Read_User_Profile(new DTO { ID = dto.Reported_ID }).ToString();
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Delete_Account_By_User_id(DTO dto)
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
        public Task<string> Read_User(ulong end_user_id)
        {//Getting Information About the End User...
            bool Nav_lock = _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.User_id == end_user_id).Select(x => x.Locked).SingleOrDefault();
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Custom).SingleOrDefault();
            byte Status = 0;
            byte Light = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Light).SingleOrDefault();
            byte Night = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Night).SingleOrDefault();
            byte CustomTheme = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Custom).SingleOrDefault();
            byte Theme = 0;
            byte LeftAligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Left).SingleOrDefault();
            byte CenterAligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Center).SingleOrDefault();
            byte RightAligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Right).SingleOrDefault();
            byte Alignment = 0;
            byte LeftTextAligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Left).SingleOrDefault();
            byte CenterTextAligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Center).SingleOrDefault();
            byte RightTextAligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Right).SingleOrDefault();
            byte TextAlignment = 0;
            ulong LoginTS = _UsersDBC.Login_Time_StampTbl.Where(x => x.User_id == end_user_id).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_id == end_user_id).Select(x => x.Logout_on).SingleOrDefault();
            ulong Created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == end_user_id).Select(x => x.Created_on).SingleOrDefault();
            string? customLbl = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Custom_lbl).SingleOrDefault();
            string? Email = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_id == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? RegionCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == end_user_id).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == end_user_id).Select(x => x.Language_code).SingleOrDefault();
            string? Avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == end_user_id).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? Avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == end_user_id).Select(x => x.Avatar_title).SingleOrDefault();
            string? DisplayName = _UsersDBC.Selected_NameTbl.Where(x => x.User_id == end_user_id).Select(x => x.Name).SingleOrDefault();
            byte? Gender = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Gender).SingleOrDefault();
            byte? birth_day = _UsersDBC.Birth_DateTbl.Where(x => x.User_id == end_user_id).Select(x => x.Day).SingleOrDefault();
            byte? birth_month = _UsersDBC.Birth_DateTbl.Where(x => x.User_id == end_user_id).Select(x => x.Month).SingleOrDefault();
            ulong? birth_year = _UsersDBC.Birth_DateTbl.Where(x => x.User_id == end_user_id).Select(x => x.Year).SingleOrDefault();
            string? FirstName = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.First_Name).SingleOrDefault();
            string? LastName = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Last_Name).SingleOrDefault();
            string? MiddleName = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Middle_Name).SingleOrDefault();
            string? MaidenName = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Maiden_Name).SingleOrDefault();
            string? Ethnicity = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Ethnicity).SingleOrDefault();

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

            obj.id = end_user_id;
            obj.email_address = Email;
            obj.display_name = DisplayName;
            obj.login_on = LoginTS;
            obj.logout_on = LogoutTS;
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
            obj.birth_day = birth_day;
            obj.birth_month = birth_month;
            obj.birth_year = birth_year;
            obj.first_name = FirstName;
            obj.last_name = LastName;
            obj.middle_name = MiddleName;
            obj.maiden_name = MaidenName;
            obj.ethnicity = Ethnicity;

            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public async Task<string> Read_User_Profile(DTO dto)
        {
            //Get Information About the End User for the client sidx.
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.Custom).SingleOrDefault();
            string? customLbl = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.ID).Select(x => x.Custom_lbl).SingleOrDefault();

            //Send Information to Client Side going below...
            byte Status = 0;

            ulong LoginTS = _UsersDBC.Login_Time_StampTbl.Where(x => x.User_id == dto.ID).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_id == dto.ID).Select(x => x.Logout_on).SingleOrDefault();
            ulong Created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == dto.ID).Select(x => x.Created_on).SingleOrDefault();

            string? Email = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_id == dto.ID).Select(x => x.Email_Address).SingleOrDefault();
            string? RegionCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == dto.ID).Select(x => x.Region_code).SingleOrDefault();
            string? LanguageCode = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == dto.ID).Select(x => x.Language_code).SingleOrDefault();
            string? Avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == dto.ID).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? Avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == dto.ID).Select(x => x.Avatar_title).SingleOrDefault();
            string? DisplayName = _UsersDBC.Selected_NameTbl.Where(x => x.User_id == dto.ID).Select(x => x.Name).SingleOrDefault();

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
        public Task<string> Read_WebSocket_Permission_Record(Websocket_Chat_PermissionDTO dto)
        {
            byte requested = _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_A_id == dto.User_A_id && x.User_B_id == dto.User_B_id).Select(x => x.Requested).SingleOrDefault();
            byte approved = _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_A_id == dto.User_A_id && x.User_B_id == dto.User_B_id).Select(x => x.Approved).SingleOrDefault();
            byte blocked = _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_A_id == dto.User_A_id && x.User_B_id == dto.User_B_id).Select(x => x.Blocked).SingleOrDefault();

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

            obj.id = dto.User_A_id;
            obj.send_to = dto.User_B_id;
            obj.message = dto.Message;

            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public async Task<string> Read_End_User_Web_Socket_Data(ulong user_id)
        {
            return await Task.FromResult(JsonSerializer.Serialize(_UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_A_id == user_id).ToList().Concat(_UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_B_id == user_id).ToList())));
        }
        public async Task<byte> Read_End_User_Selected_Status(Selected_StatusDTO dto)
        {
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).Select(x => x.Custom).SingleOrDefault();
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
        public async Task<string> Create_Reported_WebSocket_Abuse_Record(Reported_WebSocket_AbuseDTO dto) 
        {
            try
            {
                await _UsersDBC.Reported_WebSocket_AbuseTbl.AddAsync(new Reported_WebSocket_AbuseTbl
                {
                    User_id = dto.User_id,
                    Updated_on = TimeStamp,
                    Updated_by = dto.User_id,
                    Abuser = dto.Abuser,
                    Type = dto.Abuse_type,
                    Reason = dto.Reason,
                    Created_on = TimeStamp,
                    Created_by = dto.User_id,
                });
                await _UsersDBC.SaveChangesAsync();
                obj.record_created_on = TimeStamp;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Record Creation Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_Chat_Web_Socket_Permissions_Tbl(Websocket_Chat_PermissionTbl dto)
        {
            try
            {
                await _UsersDBC.Websocket_Chat_PermissionTbl.Where(x => x.User_A_id == dto.User_A_id&& x.User_B_id == dto.User_B_id).ExecuteUpdateAsync(s => s
                    .SetProperty(dto => dto.Requested, dto.Requested)
                    .SetProperty(dto => dto.Blocked, dto.Blocked)
                    .SetProperty(dto => dto.Approved, dto.Approved)
                    .SetProperty(dto => dto.Updated_on, TimeStamp)
                    .SetProperty(dto => dto.Updated_by, dto.User_A_id)
                );
                await _UsersDBC.SaveChangesAsync();
                obj.updated_on = TimeStamp;
                obj.user = dto.User_A_id;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Chat Permissions Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto)
        {
            try { 
                await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Email_Address, dto.Email_Address)
                    .SetProperty(col => col.Code, dto.Code)
                    .SetProperty(col => col.Language_Region, @$"{dto.Language}-{dto.Region}")
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, (ulong)0)
                );
                await _UsersDBC.SaveChangesAsync();

                obj.email_address = AES_RW.Process_Encryption(dto.Email_Address);
                obj.language = AES_RW.Process_Encryption(dto.Language);
                obj.region = AES_RW.Process_Encryption(dto.Region);
                obj.updated_on = AES_RW.Process_Encryption(TimeStamp.ToString());

                return JsonSerializer.Serialize(obj);            
            } catch {
                obj.error = "Server Error: Email Address Registration Failed";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_Unconfirmed_Phone(DTO dto)
        {
            try { 
                await _UsersDBC.Pending_Telephone_RegistrationTbl.Where(x => x.Phone == dto.Phone).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Country, dto.Country)
                    .SetProperty(col => col.Carrier, dto.Carrier)
                    .SetProperty(col => col.Code, dto.Code)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, 0F)
                    .SetProperty(col => col.Created_on, TimeStamp));
                await _UsersDBC.SaveChangesAsync();
                obj.Updated_on = TimeStamp;
                return JsonSerializer.Serialize(obj);            
            } catch {
                obj.error = "Server Error: Updated Phone Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Avatar(Selected_AvatarDTO dto)
        {
            try { 
                await _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Avatar_title, dto.Avatar_title)
                    .SetProperty(col => col.Avatar_url_path, dto.Avatar_url_path)
                );
                await _UsersDBC.SaveChangesAsync();
                obj.avatar_title = dto.Avatar_title;
                obj.avatar_url_path = dto.Avatar_url_path;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Avatar Failed.";
                return JsonSerializer.Serialize(obj);
            }

        }
        public async Task<string> Update_End_User_Name(Selected_NameDTO dto)
        {
            try { 
                await _UsersDBC.Selected_NameTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Name, dto.Name)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                );
                await _UsersDBC.SaveChangesAsync();
                obj.name = dto.Name;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Name Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Selected_Alignment(Selected_App_AlignmentDTO dto)
        {
            try {
                switch ((byte)dto.Alignment)
                {
                    case 0:
                        if (!_UsersDBC.Selected_App_AlignmentTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_AlignmentTbl.Count() + 1),
                                User_id = dto.User_id,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Left = 1,
                                Updated_by = dto.User_id
                            });
                        } else { //Update
                            await _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 1)
                                .SetProperty(col => col.Center, 0)
                                .SetProperty(col => col.Right, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.alignment = "left";
                        return JsonSerializer.Serialize(obj);
                    case 2:
                        if (!_UsersDBC.Selected_App_AlignmentTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_AlignmentTbl.Count() + 1),
                                User_id = dto.User_id,
                                Right = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        } else { //Update
                            await _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 0)
                                .SetProperty(col => col.Center, 0)
                                .SetProperty(col => col.Right, 1)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.alignment = "right";
                        return JsonSerializer.Serialize(obj);
                    case 1:
                        if (!_UsersDBC.Selected_App_AlignmentTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_AlignmentTbl.Count() + 1),
                                User_id = dto.User_id,
                                Center = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        } else { //Update
                            await _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 0)
                                .SetProperty(col => col.Center, 1)
                                .SetProperty(col => col.Right, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.alignment = "center";
                        return JsonSerializer.Serialize(obj);
                    default:
                        obj.alignment = "error";
                        return JsonSerializer.Serialize(obj);
                }                
            } catch {
                obj.error = "Server Error: Update Alignment Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Selected_TextAlignment(Selected_App_Text_AlignmentDTO dto)
        {
            try { 
                switch ((byte)dto.Text_alignment)
                {
                    case 0:
                        if (!_UsersDBC.Selected_App_Text_AlignmentTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_Text_AlignmentTbl.Count() + 1),
                                User_id = dto.User_id,
                                Left = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        }
                        else
                        { //Update
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 1)
                                .SetProperty(col => col.Center, 0)
                                .SetProperty(col => col.Right, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.text_alignment = "Left";
                        return JsonSerializer.Serialize(obj);
                    case 2:
                        if (!_UsersDBC.Selected_App_Text_AlignmentTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_Text_AlignmentTbl.Count() + 1),
                                User_id = dto.User_id,
                                Right = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        }
                        else
                        { //Update
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 0)
                                .SetProperty(col => col.Center, 0)
                                .SetProperty(col => col.Right, 1)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.text_alignment = "Right";
                        return JsonSerializer.Serialize(obj);
                    case 1:
                        if (!_UsersDBC.Selected_App_Text_AlignmentTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_Text_AlignmentTbl.Count() + 1),
                                User_id = dto.User_id,
                                Center = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        }
                        else
                        { //Update
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 0)
                                .SetProperty(col => col.Center, 1)
                                .SetProperty(col => col.Right, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.text_alignment = "Center";
                        return JsonSerializer.Serialize(obj);
                    default:
                        obj.text_alignment = "error";
                        return JsonSerializer.Serialize(obj);
                }            
            } catch {
                obj.error = "Server Error: Update Text Alignment Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Selected_Language(Selected_LanguageDTO dto)
        {
            try {
                if (!_UsersDBC.Selected_LanguageTbl.Any((x => x.User_id == dto.User_id)))
                {
                    await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_LanguageTbl.Count() + 1),
                        User_id = dto.User_id,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.User_id,
                        Created_by = dto.User_id
                    });
                }
                else {
                    await _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Language_code, dto.Language)
                        .SetProperty(col => col.Region_code, dto.Region)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.User_id)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                obj.language_region = @$"{dto.Language}-{dto.Region}";
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Text Alignment Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Selected_Nav_Lock(Selected_Navbar_LockDTO dto)
        {
            try { 
                await _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Updated_by, dto.User_id)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Locked, dto.Locked)
                );
                await _UsersDBC.SaveChangesAsync();
                obj.nav_lock = dto.Locked;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Text Alignment Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Selected_Status(Selected_StatusDTO dto)
        {             
            try {
                switch (dto.Online_status)
                {
                    case 0:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Offline";
                        return JsonSerializer.Serialize(obj);
                    case 1:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 0)
                            .SetProperty(col => col.Hidden, 1)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Hidden";
                        return JsonSerializer.Serialize(obj);
                    case 2:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 0)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 1)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Online";
                        return JsonSerializer.Serialize(obj);
                    case 3:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 0)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 1)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Away";
                        return JsonSerializer.Serialize(obj);
                    case 4:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 0)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 1)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Do Not Disturb";
                        return JsonSerializer.Serialize(obj);
                    case 5:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 0)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 1)
                            .SetProperty(col => col.Custom_lbl, dto.Custom_lbl)
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Custom";
                        return JsonSerializer.Serialize(obj);
                    case 10:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 1)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Hidden";
                        return JsonSerializer.Serialize(obj);
                    case 20:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 1)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Online";
                        return JsonSerializer.Serialize(obj);
                    case 30:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 1)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Away";
                        return JsonSerializer.Serialize(obj);
                    case 40:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 1)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Do Not Disturb";
                        return JsonSerializer.Serialize(obj);
                    case 50:
                        await _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 0)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 1)
                            .SetProperty(col => col.Custom_lbl, dto.Custom_lbl)
                            .SetProperty(col => col.Updated_by, dto.User_id)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                        await _UsersDBC.SaveChangesAsync();
                        obj.online_status = "Custom";
                        return JsonSerializer.Serialize(obj);
                    default:
                        obj.error = "Server Error: Update Status Unknown.";
                        return JsonSerializer.Serialize(obj);
                }
            } catch {
                obj.error = "Server Error: Update Status Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Selected_Theme(Selected_ThemeDTO dto)
        { 
            try { 
                switch (dto.Theme)
                {
                    case 0:
                        if (!_UsersDBC.Selected_ThemeTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                                User_id = dto.User_id,
                                Light = 1,
                                Updated_on = TimeStamp,
                                Created_by = dto.User_id,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        } else { //Update
                            await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Light, 1)
                                .SetProperty(col => col.Night, 0)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.theme = "Light";
                        return JsonSerializer.Serialize(obj);
                    case 1:
                        if (!_UsersDBC.Selected_ThemeTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                                User_id = dto.User_id,
                                Night = 1,
                                Updated_on = TimeStamp,
                                Created_by = dto.User_id,
                                Created_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        }
                        else
                        { //Update
                            await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Light, 0)
                                .SetProperty(col => col.Night, 1)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.theme = "Night";
                        return JsonSerializer.Serialize(obj);
                    case 2:
                        if (!_UsersDBC.Selected_ThemeTbl.Any(x => x.User_id == dto.User_id))
                        {//Insert
                            await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                                User_id = dto.User_id,
                                Night = 1,
                                Created_by = dto.User_id,
                                Created_on = TimeStamp,
                                Updated_on = TimeStamp,
                                Updated_by = dto.User_id
                            });
                        } else { //Update
                            await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.User_id)
                                .SetProperty(col => col.Light, 0)
                                .SetProperty(col => col.Night, 0)
                                .SetProperty(col => col.Custom, 1)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        obj.theme = "Custom";
                        return JsonSerializer.Serialize(obj);
                    default:
                        obj.theme = "error";
                        return JsonSerializer.Serialize(obj);
                }
            } catch {
                obj.error = "Server Error: Update Theme Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Password(Login_PasswordDTO dto)
        {
            try { 
                await _UsersDBC.Login_PasswordTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Password, Create_Salted_Hash_String(Encoding.UTF8.GetBytes($"{dto.Password}"), Encoding.UTF8.GetBytes($"{dto.Password}MPCSalt")).Result)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                );
                await _UsersDBC.SaveChangesAsync();
                obj.updated_on = TimeStamp;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Password Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Login(Login_Time_StampDTO dto)
        {
            try { 
                if(!_UsersDBC.Login_Time_StampTbl.Any(x=>x.User_id == dto.User_id))
                    await _UsersDBC.Login_Time_StampTbl.AddAsync(new Login_Time_StampTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Login_Time_StampTbl.Count() + 1),
                        User_id = dto.User_id,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.User_id,
                        Created_by = dto.User_id
                    });


                await _UsersDBC.Login_Time_StampTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Updated_by, dto.User_id)
                    .SetProperty(col => col.Login_on, TimeStamp)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                );
                await _UsersDBC.SaveChangesAsync();

                obj.login_on = TimeStamp;
                obj.token = Create_Jwt_Token(@$"{dto.User_id}").Result;
                obj.token_expire = DateTime.UtcNow.AddMinutes(TokenExpireTime);
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;            
            } catch {
                obj.error = "Server Error: Update Login Failed.";
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
            }
        }
        public async Task<string> Update_End_User_Logout(ulong id)
        {
            if (_UsersDBC.Logout_Time_StampTbl.Any(x=>x.User_id == id))
            {//update
                await _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_id == id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Logout_on, TimeStamp)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, id)
                );
                await _UsersDBC.SaveChangesAsync();
            }
            else
            {//insert
                await _UsersDBC.Logout_Time_StampTbl.AddAsync(new Logout_Time_StampTbl
                {
                    User_id = id,
                    Logout_on = TimeStamp,
                    Updated_by = id,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp
                });
                await _UsersDBC.SaveChangesAsync();
            }
            obj.id = id;
            obj.logout_on = TimeStamp;
            return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
        }
        public Task<string> Read_Users()
        {
            obj.logoutsTS = _UsersDBC.Logout_Time_StampTbl.Select(x => x).ToList();
            obj.loginsTS = _UsersDBC.Login_Time_StampTbl.Select(x => x).ToList();
            obj.display_names = _UsersDBC.Selected_NameTbl.Select(x => x).ToList();
            obj.avatars = _UsersDBC.Selected_AvatarTbl.Select(x => x).ToList();
            obj.languages = _UsersDBC.Selected_LanguageTbl.Select(x => x).ToList();
            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public Task<bool> ID_Exists_In_Users_Tbl(ulong user_id) {
            return Task.FromResult(_UsersDBC.User_IDsTbl.Any(x => x.ID == user_id));
        }
        public async Task<bool> Email_Exists_In_Pending_Email_RegistrationTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Email_Address == email_address));
        }
        public async Task<bool> Confirmation_Code_Exists_In_Pending_Email_Address_RegistrationTbl (string Code)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Code == Code));
        }
        public async Task<bool> Telephone_Exists_In_Login_Telephone_Tbl(string telephone_number)
        {
            return await Task.FromResult(_UsersDBC.Login_TelephoneTbl.Any(x => x.Phone == telephone_number));
        }
        public async Task<bool> Phone_Exists_In_Telephone_Not_Confirmed_Tbl(string telephone_number)
        {
            return await Task.FromResult(_UsersDBC.Pending_Telephone_RegistrationTbl.Any(x => x.Phone == telephone_number));
        }
        public async Task<ulong> Read_User_ID_By_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.Email_Address == email_address).Select(x => x.User_id).SingleOrDefault());
        }
        public async Task<string?> Read_User_Email_By_ID(ulong id)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.User_id == id).Select(x => x.Email_Address).SingleOrDefault());
        }
        public async Task<ulong> Read_User_ID_By_JWToken(string jwtToken)
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
            HashAlgorithm algorithm = SHA256.Create();

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
        public async Task<byte[]?> Read_User_Password_Hash_By_ID(ulong user_id)
        {
            return await Task.FromResult(_UsersDBC.Login_PasswordTbl.Where(user => user.User_id == user_id).Select(user => user.Password).SingleOrDefault());
        }
        public async Task<string> Create_Jwt_Token(string id)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, $@"{id}"),
                new Claim(ClaimTypes.Role, "MPC-End-User"),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
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
        public async Task<string> Update_End_User_First_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_id == dto.User_id))
            {//Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_id = dto.User_id,
                    First_Name = dto.First_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            {//Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.First_Name, dto.First_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.User_id;
            obj.first_name = dto.First_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Last_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_id == dto.User_id))
            {//Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_id = dto.User_id,
                    Last_Name = dto.Last_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            {//Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Last_Name, dto.Last_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.User_id;
            obj.last_name = dto.Last_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Middle_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_id == dto.User_id))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_id = dto.User_id,
                    Middle_Name = dto.Middle_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                     .SetProperty(col => col.Middle_Name, dto.Middle_name)
                     .SetProperty(col => col.Updated_on, TimeStamp)
                     .SetProperty(col => col.Updated_by, dto.User_id)
                 );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.User_id;
            obj.middle_name = dto.Middle_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Maiden_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_id == dto.User_id))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_id = dto.User_id,
                    Maiden_Name = dto.Maiden_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Maiden_Name, dto.Maiden_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.User_id;
            obj.maiden_name = dto.Maiden_name;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Gender(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_id == dto.User_id))
            {//Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_id = dto.User_id,
                    Gender = dto.Gender,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            { //Update

                await _UsersDBC.IdentityTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Gender, dto.Gender)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.User_id;
            obj.gender = dto.Gender;
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Ethnicity(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_id == dto.User_id))
            { //Insert
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_id = dto.User_id,
                    Ethnicity = dto.Ethnicity,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            { //Update
                await _UsersDBC.IdentityTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Ethnicity, dto.Ethnicity)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            obj.id = dto.User_id;
            obj.ethnicity = dto.Ethnicity;

            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Birth_Date(Birth_DateDTO dto)
        {
            if (!_UsersDBC.Birth_DateTbl.Any(x => x.User_id == dto.User_id))
            { //Insert
                await _UsersDBC.Birth_DateTbl.AddAsync(new Birth_DateTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Birth_DateTbl.Count() + 1),
                    User_id = dto.User_id,
                    Month = dto.Month,
                    Day = dto.Day,
                    Year = dto.Year,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id
                });
            }
            else
            { //Update
                await _UsersDBC.Birth_DateTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Month, dto.Month)
                    .SetProperty(col => col.Day, dto.Day)
                    .SetProperty(col => col.Year, dto.Year)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.User_id)
                );
            }
            await _UsersDBC.SaveChangesAsync();
            obj.id = dto.User_id;
            obj.birth_month = dto.Month;
            obj.birth_day = dto.Day;
            obj.birth_year = dto.Year;
            return JsonSerializer.Serialize(obj);
        }
        public void Create_Chat_WebSocket_Log_Records(Websocket_Chat_PermissionDTO dto)
        {
            _UsersDBC.Websocket_Chat_PermissionTbl.AddAsync(new Websocket_Chat_PermissionTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Websocket_Chat_PermissionTbl.Count() + 1),
                User_A_id = dto.User_id,
                User_B_id = dto.User_B_id,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.User_id,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            _UsersDBC.SaveChangesAsync();

            _UsersDBC.Websocket_Chat_PermissionTbl.AddAsync(new Websocket_Chat_PermissionTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Websocket_Chat_PermissionTbl.Count() + 1),
                User_A_id = dto.User_B_id,//Swapped so we are to create the record for the other user.
                User_B_id = dto.User_id,//Both Users have a permission record now for eachother.
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.User_id,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            _UsersDBC.SaveChangesAsync();
        }
    }
}
