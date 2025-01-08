using System.Dynamic;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Integration;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.BirthDate;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Controllers;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Phone;
using mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Phone;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Telephone;
using mpc_dotnetc_user_server.Models.Users.Authentication.WebSocket_Chat;
using mpc_dotnetc_user_server.Models.Users.Authentication.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.Reported;
using mpc_dotnetc_user_server.Models.Users.Notification.Email;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Authentication.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Authentication.Account_Groups;
using mpc_dotnetc_user_server.Controllers.Users.JWT;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class UsersRepository : IUsersRepository
    {
        private readonly ulong TimeStamp = Convert.ToUInt64(DateTimeOffset.Now.ToUnixTimeSeconds());//GMT Time
        private dynamic obj = new ExpandoObject();
        private readonly UsersDBC _UsersDBC;

        AES AES = new AES();

        public UsersRepository(UsersDBC UsersDBC)
        {
            _UsersDBC = UsersDBC;
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
            obj.id = AES.Process_Encryption(ID_Record.ID.ToString());

            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, TimeStamp)
                .SetProperty(col => col.Updated_on, TimeStamp)
                .SetProperty(col => col.Updated_by, ID_Record.ID)
                .SetProperty(col => col.Deleted_by, ID_Record.ID)
                .SetProperty(col=>col.Client_time, ulong.Parse(dto.Client_time))
                .SetProperty(col=>col.Server_Port, dto.Server_Networking_Port)
                .SetProperty(col=>col.Server_IP, dto.Server_Networking_IP_Address)
                .SetProperty(col=>col.Client_IP, dto.Client_Networking_IP_Address)
                .SetProperty(col=>col.Client_Port, dto.Client_Networking_Port)
            );
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address,
                Updated_on = TimeStamp,
                Updated_by = (ulong)0,
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = TimeStamp,
                Created_by = ID_Record.ID,
                Code = dto.Code,
                Client_IP = dto.Client_Networking_IP_Address,
                Client_Port = dto.Client_Networking_Port,
                Server_IP = dto.Server_Networking_IP_Address,
                Server_Port = dto.Server_Networking_Port,
                Client_time = ulong.Parse(dto.Client_time)
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

            obj.email_address = AES.Process_Encryption(dto.Email_Address);

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                User_id = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region
            });
            obj.language = AES.Process_Encryption(dto.Language);
            obj.region = AES.Process_Encryption(dto.Region);

            await Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO
            {
                User_id = ID_Record.ID,
                Alignment = dto.Alignment,
            });
            obj.alignment = AES.Process_Encryption($"{dto.Alignment}");
            
            await Update_End_User_Selected_Nav_Lock(new Selected_Navbar_LockDTO
            {
                User_id = ID_Record.ID,
                Locked = dto.Nav_lock,
            });
            obj.nav_lock = AES.Process_Encryption($"{dto.Nav_lock}");

            await Update_End_User_Selected_TextAlignment(new Selected_App_Text_AlignmentDTO
            {
                User_id = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });
            obj.text_alignment = AES.Process_Encryption($"{dto.Text_alignment}"); ;

            await Update_End_User_Selected_Theme(new Selected_ThemeDTO
            {
                User_id = ID_Record.ID,
                Theme = dto.Theme
            });
            obj.theme = AES.Process_Encryption($"{dto.Theme}");

            await Update_End_User_Account_Roles(new Account_RolesDTO
            {
                User_id = ID_Record.ID,
                Roles = "User"
            });
            obj.roles = AES.Process_Encryption(JsonSerializer.Serialize("User"));

            await Update_End_User_Account_Groups(new Account_GroupsDTO
            {
                User_id = ID_Record.ID,
                Groups = "0"
            });
            obj.groups = AES.Process_Encryption(JsonSerializer.Serialize("0"));

            await Update_End_User_Account_Type(new Account_TypeDTO
            {
                User_id = ID_Record.ID,
                Type = 1
            });
            obj.account_type = AES.Process_Encryption("1");

            await Update_End_User_Login_Time_Stamp(new Login_Time_StampDTO
            {
                User_id = ID_Record.ID,
                Login_on = TimeStamp,
                Client_time = ulong.Parse(dto.Client_time),
                Location = dto.Location
            });

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_StampDTO
            {
                User_id = ID_Record.ID,
                Login_on = TimeStamp,
                Client_time = ulong.Parse(dto.Client_time),
                Location = dto.Location,
                Client_Networking_Port = dto.Client_Networking_Port,
                Client_Networking_IP_Address = dto.Client_Networking_IP_Address,
                Server_Networking_Port = dto.Server_Networking_Port,
                Server_Networking_IP_Address = dto.Server_Networking_IP_Address
            });

            obj.token = JWT.Create_Email_Account_Token(new JWT_DTO { 
                User_id = ID_Record.ID,
                User_groups = "0",
                User_roles = "User",
                Account_type = 1,
                Email_address = dto.Email_Address
            }).Result;

            string time = TimeStamp.ToString();
            obj.created_on = AES.Process_Encryption(time);
            obj.login_on = AES.Process_Encryption(time);
            obj.location = AES.Process_Encryption(dto.Location);
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Update_End_User_Account_Groups(Account_GroupsDTO dto)
        {
            try
            {
                if (!_UsersDBC.Account_GroupsTbl.Any(x => x.User_id == dto.User_id))
                {//Insert
                    await _UsersDBC.Account_GroupsTbl.AddAsync(new Account_GroupsTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Account_GroupsTbl.Count() + 1),
                        User_id = dto.User_id,
                        Groups = dto.Groups,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.User_id
                    });
                }
                else
                {//Update
                    await _UsersDBC.Account_GroupsTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Groups, dto.Groups)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.User_id)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                obj.groups = dto.Groups;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update Account Groups Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Account_Roles(Account_RolesDTO dto)
        {
            try
            {
                if (!_UsersDBC.Account_RolesTbl.Any(x => x.User_id == dto.User_id))
                {//Insert
                    await _UsersDBC.Account_RolesTbl.AddAsync(new Account_RolesTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Account_RolesTbl.Count() + 1),
                        User_id = dto.User_id,
                        Roles = dto.Roles,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.User_id
                    });
                } else {//Update
                    await _UsersDBC.Account_RolesTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Roles, dto.Roles)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.User_id)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                obj.roles = dto.Roles;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Update End User Roles Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Create_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.AddAsync(new Pending_Email_RegistrationTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Pending_Email_RegistrationTbl.Count() + 1),
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Client_IP = dto.Client_Networking_IP_Address,
                Client_Port = dto.Client_Networking_Port,
                Server_IP = dto.Server_Networking_IP_Address,
                Server_Port = dto.Server_Networking_Port,
                Language_Region = $"{dto.Language}-{dto.Region}",
                Email_Address = dto.Email_Address,
                Location = dto.Location,
                Client_time = ulong.Parse(dto.Client_time),
                Code = dto.Code
            });
            await _UsersDBC.SaveChangesAsync();

            obj.email_address = AES.Process_Encryption(dto.Email_Address);
            obj.code = AES.Process_Encryption(dto.Code);
            obj.language = AES.Process_Encryption(dto.Language);
            obj.region = AES.Process_Encryption(dto.Region);
            obj.created_on = AES.Process_Encryption(TimeStamp.ToString());
            return JsonSerializer.Serialize(obj);
        }        
        public async Task<bool> Email_Exists_In_Login_Email_AddressTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Any(x => x.Email_Address == email_address));
        }
        public async Task<bool> Create_Contact_Us_Record(Contact_UsDTO dto)
        {
            await _UsersDBC.Contact_UsTbl.AddAsync(new Contact_UsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Contact_UsTbl.Count() + 1),
                USER_ID = dto.ID,
                Subject_Line = dto.Subject_line,
                Summary = dto.Summary,
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
        public async Task<bool> Create_Website_Bug_Record(Reported_Website_BugDTO dto)
        {
            await _UsersDBC.Reported_Website_BugTbl.AddAsync(new Reported_Website_BugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Website_BugTbl.Count() + 1),
                USER_ID = dto.ID,
                URL = dto.URL,
                Detail = dto.Detail,
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
        public async Task<bool> Create_Discord_Bot_Bug_Record(Reported_Discord_Bot_BugDTO obj)
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
        public async Task<bool> Create_Comment_Box_Record(Comment_BoxDTO dto)
        {
            await _UsersDBC.Comment_BoxTbl.AddAsync(new Comment_BoxTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Comment_BoxTbl.Count() + 1),
                USER_ID = dto.ID,
                Comment = dto.Comment,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Broken_Link_Record(Reported_Broken_LinkDTO obj)
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
        public async Task<string> Create_Reported_User_Profile_Record(Reported_ProfileDTO dto)
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
            obj.read_reported_profile = Read_User_Profile_By_ID(dto.Reported_ID).ToString();
            return JsonSerializer.Serialize(obj);
        }
        public async Task<string> Delete_Account_By_User_id(Delete_UserDTO dto)
        {
            await _UsersDBC.User_IDsTbl.Where(x => x.ID == dto.Target_User).ExecuteUpdateAsync(s => s
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted, 1)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_by, dto.ID)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Created_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_by, dto.ID)
            );
            await _UsersDBC.SaveChangesAsync();
            obj.deleted_by = dto.ID;
            obj.target_user = dto.Target_User;
            return JsonSerializer.Serialize(obj);
        }
        public Task<string> Read_User(ulong end_user_id)
        {
            bool nav_lock = _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.User_id == end_user_id).Select(x => x.Locked).SingleOrDefault();
            byte account_type = _UsersDBC.Account_TypeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Type).SingleOrDefault();
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Custom).SingleOrDefault();
            byte status_type = 0;
            byte light = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Light).SingleOrDefault();
            byte night = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Night).SingleOrDefault();
            byte custom_theme = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_id == end_user_id).Select(x => x.Custom).SingleOrDefault();
            byte theme_type = 0;
            byte left_aligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Left).SingleOrDefault();
            byte center_aligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Center).SingleOrDefault();
            byte right_aligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Right).SingleOrDefault();
            byte alignment_type = 0;
            byte left_text_aligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Left).SingleOrDefault();
            byte center_text_aligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Center).SingleOrDefault();
            byte right_text_aligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_id == end_user_id).Select(x => x.Right).SingleOrDefault();
            byte text_alignment_type = 0;
            ulong login_timestamp = _UsersDBC.Login_Time_StampTbl.Where(x => x.User_id == end_user_id).Select(x => x.Login_on).SingleOrDefault();
            ulong logout_timestamp = _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_id == end_user_id).Select(x => x.Logout_on).SingleOrDefault();
            ulong created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == end_user_id).Select(x => x.Created_on).SingleOrDefault();
            string? customLbl = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == end_user_id).Select(x => x.Custom_lbl).SingleOrDefault();
            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_id == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? region_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == end_user_id).Select(x => x.Region_code).SingleOrDefault();
            string? language_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == end_user_id).Select(x => x.Language_code).SingleOrDefault();
            string? avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == end_user_id).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == end_user_id).Select(x => x.Avatar_title).SingleOrDefault();
            string? display_name = _UsersDBC.Selected_NameTbl.Where(x => x.User_id == end_user_id).Select(x => x.Name).SingleOrDefault();
            byte? gender = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Gender).SingleOrDefault();
            byte? birth_day = _UsersDBC.Birth_DateTbl.Where(x => x.User_id == end_user_id).Select(x => x.Day).SingleOrDefault();
            byte? birth_month = _UsersDBC.Birth_DateTbl.Where(x => x.User_id == end_user_id).Select(x => x.Month).SingleOrDefault();
            ulong? birth_year = _UsersDBC.Birth_DateTbl.Where(x => x.User_id == end_user_id).Select(x => x.Year).SingleOrDefault();
            string? first_name = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.First_Name).SingleOrDefault();
            string? last_name = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Last_Name).SingleOrDefault();
            string? middle_name = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Middle_Name).SingleOrDefault();
            string? maiden_name = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Maiden_Name).SingleOrDefault();
            string? ethnicity = _UsersDBC.IdentityTbl.Where(x => x.User_id == end_user_id).Select(x => x.Ethnicity).SingleOrDefault();

            if (status_offline == 1)
                status_type = 0;
            if (status_hidden == 1)
                status_type = 1;
            if (status_online == 1)
                status_type = 2;
            if (status_away == 1)
                status_type = 3;
            if (status_dnd == 1)
                status_type = 4;
            if (status_custom == 1)
                status_type = 5;
            if (light == 1)
                theme_type = 0;
            if (night == 1)
                theme_type = 1;
            if (custom_theme == 1)
                theme_type = 2;
            if (left_aligned == 1)
                alignment_type = 0;
            if (center_aligned == 1)
                alignment_type = 1;
            if (right_aligned == 1)
                alignment_type = 2;
            if (left_text_aligned == 1)
                text_alignment_type = 0;
            if (center_text_aligned == 1)
                text_alignment_type = 1;
            if (right_text_aligned == 1)
                text_alignment_type = 2;

            obj.id = end_user_id;
            obj.account_type = account_type;
            obj.email_address = email_address;
            obj.display_name = display_name;
            obj.login_on = login_timestamp;
            obj.logout_on = logout_timestamp;
            obj.language = @$"{language_code}-{region_code}";
            obj.online_status = status_type;
            obj.custom_lbl = customLbl;
            obj.created_on = created_on;
            obj.avatar_url_path = avatar_url_path;
            obj.avatar_title = avatar_title;
            obj.theme = theme_type;
            obj.alignment = alignment_type;
            obj.text_alignment = text_alignment_type;
            obj.gender = gender;
            obj.birth_day = birth_day;
            obj.birth_month = birth_month;
            obj.birth_year = birth_year;
            obj.first_name = first_name;
            obj.last_name = last_name;
            obj.middle_name = middle_name;
            obj.maiden_name = maiden_name;
            obj.ethnicity = ethnicity;

            return Task.FromResult(JsonSerializer.Serialize(obj));
        }
        public async Task<string> Read_User_Profile_By_ID(ulong user_id)
        {
            //Get Information About the End User for the client sidx.
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.Custom).SingleOrDefault();
            string? custom_label = _UsersDBC.Selected_StatusTbl.Where(x => x.User_id == user_id).Select(x => x.Custom_lbl).SingleOrDefault();

            //Send Information to Client Side going below...
            byte status_code = 0;

            ulong LoginTS = _UsersDBC.Login_Time_StampTbl.Where(x => x.User_id == user_id).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_id == user_id).Select(x => x.Logout_on).SingleOrDefault();
            ulong created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == user_id).Select(x => x.Created_on).SingleOrDefault();

            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_id == user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? region_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == user_id).Select(x => x.Region_code).SingleOrDefault();
            string? language_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_id == user_id).Select(x => x.Language_code).SingleOrDefault();
            string? avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == user_id).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_id == user_id).Select(x => x.Avatar_title).SingleOrDefault();
            string? name = _UsersDBC.Selected_NameTbl.Where(x => x.User_id == user_id).Select(x => x.Name).SingleOrDefault();

            if (status_offline == 1)
                status_code = 0;
            if (status_hidden == 1)
                status_code = 1;
            if (status_online == 1)
                status_code = 2;
            if (status_away == 1)
                status_code = 3;
            if (status_dnd == 1)
                status_code = 4;
            if (status_custom == 1)
                status_code = 5;

            obj.id = user_id;
            obj.email_address = email_address;
            obj.name = name;
            obj.login_on = LoginTS;
            obj.logout_on = LogoutTS;
            obj.language = @$"{language_code}-{region_code}";
            obj.online_status = status_code;
            obj.custom_lbl = custom_label;
            obj.created_on = created_on;
            obj.avatar_url_path = avatar_url_path;
            obj.avatar_title = avatar_title;

            return await Task.FromResult(JsonSerializer.Serialize(obj));
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
        public async Task<string> Insert_Pending_Email_Registration_History_Record(Pending_Email_Registration_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Pending_Email_Registration_HistoryTbl.AddAsync(new Pending_Email_Registration_HistoryTbl
                {
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Client_IP = dto.Client_Networking_IP_Address,
                    Client_Port = dto.Client_Networking_Port,
                    Server_IP = dto.Server_Networking_IP_Address,
                    Server_Port = dto.Server_Networking_Port,
                    Language_Region = $"{dto.Language}-{dto.Region}",
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Client_time = ulong.Parse(dto.Client_time),
                    Code = dto.Code
                });
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Email Address Registration Failed";
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

                obj.email_address = AES.Process_Encryption(dto.Email_Address);
                obj.language = AES.Process_Encryption(dto.Language);
                obj.region = AES.Process_Encryption(dto.Region);
                obj.updated_on = AES.Process_Encryption(TimeStamp.ToString());

                return JsonSerializer.Serialize(obj);            
            } catch {
                obj.error = "Server Error: Email Address Registration Failed";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Create_Reported_Email_Registration_Record(Reported_Email_RegistrationDTO dto) 
        {
            try
            {
                await _UsersDBC.Reported_Email_RegistrationTbl.AddAsync(new Reported_Email_RegistrationTbl
                {
                    User_ID = dto.User_ID,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Client_IP = dto.Client_Networking_IP_Address,
                    Client_Port = dto.Client_Networking_Port,
                    Server_IP = dto.Server_Networking_IP_Address,
                    Server_Port = dto.Server_Networking_Port,
                    Language_Region = $"{dto.Language}-{dto.Region}",
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Client_time = ulong.Parse(dto.Client_time)
                });
                await _UsersDBC.SaveChangesAsync();
                obj.recorded_on = TimeStamp;
                return JsonSerializer.Serialize(obj);
            } catch {
                obj.error = "Server Error: Record Creation Failed.";
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
                        return JsonSerializer.Serialize(obj);
                    default:
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
                        return JsonSerializer.Serialize(obj);
                    default:
                        return JsonSerializer.Serialize(obj);
                }            
            } catch {
                obj.error = "Server Error: Update Text Alignment Failed.";
                return JsonSerializer.Serialize(obj);
            }
        }
        public async Task<string> Update_End_User_Account_Type(Account_TypeDTO dto)
        {
            try {
                if (!_UsersDBC.Account_TypeTbl.Any(x => x.User_id == dto.User_id)) {//Insert
                    await _UsersDBC.Account_TypeTbl.AddAsync(new Account_TypeTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Account_TypeTbl.Count() + 1),
                        User_id = dto.User_id,
                        Type = dto.Type,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.User_id
                    });
                } else {//Update
                    await _UsersDBC.Account_TypeTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Type, dto.Type)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.User_id)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                obj.text_alignment = "error";//...
                return JsonSerializer.Serialize(obj);
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
                obj.region = dto.Region;
                obj.language = dto.Language;
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
        public async Task<string> Update_End_User_Login_Time_Stamp(Login_Time_StampDTO dto)
        {
            try {
                if (!_UsersDBC.Login_Time_StampTbl.Any(x => x.User_id == dto.User_id))
                {
                    await _UsersDBC.Login_Time_StampTbl.AddAsync(new Login_Time_StampTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Login_Time_StampTbl.Count() + 1),
                        User_id = dto.User_id,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.User_id,
                        Created_by = dto.User_id,
                        Login_on = TimeStamp,
                        Location = dto.Location,
                        Client_time = dto.Client_time
                    });
                } else {
                    await _UsersDBC.Login_Time_StampTbl.Where(x => x.User_id == dto.User_id).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Updated_by, dto.User_id)
                    .SetProperty(col => col.Login_on, TimeStamp)
                    .SetProperty(col => col.Location, dto.Location)
                    .SetProperty(col => col.Client_time, dto.Client_time)
                    .SetProperty(col => col.Updated_on, TimeStamp));
                    await _UsersDBC.SaveChangesAsync();
                }
                obj.login_on = TimeStamp;
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;            
            } catch {
                obj.error = "Server Error: Update Login Failed.";
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
            }
        }
        public async Task<string> Insert_End_User_Login_Time_Stamp_History(Login_Time_StampDTO dto)
        {
            try
            {
                await _UsersDBC.Login_Time_Stamp_HistoryTbl.AddAsync(new Login_Time_Stamp_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Login_Time_Stamp_HistoryTbl.Count() + 1),
                    User_id = dto.User_id,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.User_id,
                    Created_by = dto.User_id,
                    Login_on = TimeStamp,
                    Location = dto.Location,
                    Client_IP = dto.Client_Networking_IP_Address,
                    Client_Port = dto.Client_Networking_Port,
                    Server_IP = dto.Server_Networking_IP_Address,
                    Server_Port = dto.Server_Networking_Port,
                    Client_time = dto.Client_time
                });
                await _UsersDBC.SaveChangesAsync();
                obj.insert = "completed";
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
            } catch {
                obj.error = "Server Error: Login Time Stamp Failed.";
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
            }
        }
        public async Task<string> Insert_Report_Email_RegistrationTbl(Report_Email_RegistrationDTO dto)
        {
            try
            {
                await _UsersDBC.Reported_Email_RegistrationTbl.AddAsync(new Reported_Email_RegistrationTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Reported_Email_RegistrationTbl.Count() + 1),
                    User_ID = dto.User_id,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Created_by = 0,
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Client_IP = dto.Client_Networking_IP_Address,
                    Client_Port = dto.Client_Networking_Port,
                    Server_IP = dto.Server_Networking_IP_Address,
                    Server_Port = dto.Server_Networking_Port,
                    Client_time = ulong.Parse(dto.Client_time)
                });
                await _UsersDBC.SaveChangesAsync();
                obj.insert = "completed";
                return Task.FromResult(JsonSerializer.Serialize(obj)).Result;
            }
            catch
            {
                obj.error = "Server Error: Reported Email Registration Failed.";
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
            obj.account_types = _UsersDBC.Account_TypeTbl.Select(x => x).ToList();
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
        public async Task<string> Create_Integration_Twitch_Record(Integration_TwitchDTO dto)
        {
            obj.id = null;
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

    }
}
