using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Friends;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Password_Change;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;
using System.Text;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository
{
    public class Users_Repository_Update : IUsers_Repository_Update
    {
        private long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private readonly Users_Database_Context _UsersDBC;
        private readonly Constants _Constants;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly IPassword Password;

        public Users_Repository_Update(
            Users_Database_Context Users_Database_Context,
            Constants constants,
            IAES aes,
            IJWT jwt,
            IPassword password
        )
        {
            _UsersDBC = Users_Database_Context;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Password = password;
        }
        public async Task<string> Update_Chat_Web_Socket_Permissions(WebSocket_Chat_Permission dto)
        {
            try
            {
                var websocket_permission_record_exists_in_database = await _UsersDBC.WebSocket_Chat_PermissionTbl
                    .Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID || x.Participant_ID == dto.End_User_ID && x.End_User_ID == x.Participant_ID)
                    .FirstOrDefaultAsync();

                if (websocket_permission_record_exists_in_database == null)
                {
                    await _UsersDBC.WebSocket_Chat_PermissionTbl.AddAsync(new WebSocket_Chat_PermissionTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Participant_ID = dto.Participant_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Requested = dto.Requested,
                        Blocked = dto.Blocked,
                        Approved = dto.Approved
                    });
                }
                else
                {
                    websocket_permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    websocket_permission_record_exists_in_database.Updated_on = TimeStamp();
                    websocket_permission_record_exists_in_database.Deleted = false;
                    websocket_permission_record_exists_in_database.Blocked = dto.Blocked;
                    websocket_permission_record_exists_in_database.Approved = dto.Approved;
                    websocket_permission_record_exists_in_database.Requested = dto.Requested;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    participant_id = dto.Participant_ID,
                    updated_on = TimeStamp(),
                    updated_by = dto.End_User_ID
                });
            }
            catch
            {
                return "Server Error: Update Chat Permissions Failed.";
            }
        }
        public async Task<string> Update_Pending_Email_Registration_Record(Pending_Email_Registration dto)
        {
            try
            {
                await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Email_Address, dto.Email_Address)
                    .SetProperty(col => col.Code, dto.Code)
                    .SetProperty(col => col.Language_Region, @$"{dto.Language}-{dto.Region}")
                    .SetProperty(col => col.Updated_on, TimeStamp())
                    .SetProperty(col => col.Updated_by, 0)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    email_address = dto.Email_Address,
                    language = dto.Language,
                    region = dto.Region,
                    updated_on = TimeStamp(),
                });
            }
            catch
            {
                return "Server Error: Email Address Registration Failed";
            }
        }
        public async Task<string> Update_Friend_Permissions(Friends_Permission dto)
        {
            try
            {
                var permission_record_exists_in_database = await _UsersDBC.Friends_PermissionTbl.Where(x =>
                    (x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID) ||
                    (x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID)
                ).FirstOrDefaultAsync();

                if (permission_record_exists_in_database == null)
                {
                    await _UsersDBC.Friends_PermissionTbl.AddAsync(new Friends_PermissionTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Participant_ID = dto.Participant_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Requested = dto.Requested,
                        Blocked = dto.Blocked,
                        Approved = dto.Approved
                    });
                }
                else if (dto.Unblock)
                {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Deleted = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                }
                else if (permission_record_exists_in_database.Blocked)
                {
                    return await Task.FromResult(JsonSerializer.Serialize(new
                    {
                        participant_id = dto.Participant_ID,
                        requested = false,
                        blocked = true,
                        approved = false,
                        time_stamped = TimeStamp()
                    }));
                }
                else if (dto.Blocked)
                {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = true;
                    permission_record_exists_in_database.Deleted = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                }
                else if (permission_record_exists_in_database.Deleted)
                {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = true;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Deleted = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                }
                else if (dto.Deleted)
                {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                    permission_record_exists_in_database.Deleted = true;
                    permission_record_exists_in_database.Deleted_on = TimeStamp();
                    permission_record_exists_in_database.Deleted_by = dto.End_User_ID;
                }
                else if (dto.Requested)
                {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = true;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                }
                else if (dto.Approved)
                {
                    permission_record_exists_in_database.Approved = true;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                }

                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    end_user_id = dto.End_User_ID,
                    participant_id = dto.Participant_ID,
                    requested = dto.Requested,
                    blocked = dto.Blocked,
                    approved = dto.Approved,
                    time_stamped = TimeStamp()
                }));
            }
            catch
            {
                return Task.FromResult(JsonSerializer.Serialize("Friend Request Permission Failed.")).Result;
            }
        }
        public async Task<string> Update_End_User_Avatar(Selected_Avatar dto)
        {
            try
            {
                var avatar_url_path_record = await _UsersDBC.Selected_AvatarTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (avatar_url_path_record == null)
                {
                    await _UsersDBC.Selected_AvatarTbl.AddAsync(new Selected_AvatarTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Avatar_url_path = dto.Avatar_url_path,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    avatar_url_path_record.End_User_ID = dto.End_User_ID;
                    avatar_url_path_record.Avatar_url_path = dto.Avatar_url_path;
                    avatar_url_path_record.Updated_on = TimeStamp();
                    avatar_url_path_record.Created_by = dto.End_User_ID;
                    avatar_url_path_record.Created_on = TimeStamp();
                    avatar_url_path_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    avatar_url_path = dto.Avatar_url_path
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Avatar_Title(Selected_Avatar_Title dto)
        {
            try
            {
                var avatar_title_record = await _UsersDBC.Selected_Avatar_TitleTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (avatar_title_record == null)
                {
                    await _UsersDBC.Selected_Avatar_TitleTbl.AddAsync(new Selected_Avatar_TitleTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Avatar_title = dto.Avatar_title,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    avatar_title_record.End_User_ID = dto.End_User_ID;
                    avatar_title_record.Avatar_title = dto.Avatar_title;
                    avatar_title_record.Updated_on = TimeStamp();
                    avatar_title_record.Created_by = dto.End_User_ID;
                    avatar_title_record.Created_on = TimeStamp();
                    avatar_title_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    avatar_title = dto.Avatar_title
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Name(Selected_Name dto)
        {
            try
            {
                await _UsersDBC.Selected_NameTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Name, dto.Name)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Updated_on, TimeStamp())
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    name = dto.Name
                });
            }
            catch
            {
                return "Server Error: Update Name Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Alignment(Selected_App_Alignment dto)
        {
            try
            {
                var alignment_record = await _UsersDBC.Selected_App_AlignmentTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                bool left = false, center = false, right = false;

                switch (dto.Alignment)
                {
                    case 0: left = true; break;
                    case 1: center = true; break;
                    case 2: right = true; break;
                    default: return "Server Error: Invalid alignment value.";
                }

                if (alignment_record == null)
                {
                    await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Left = left,
                        Center = center,
                        Right = right,
                        Created_on = TimeStamp(),
                        Updated_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    alignment_record.Left = left;
                    alignment_record.Center = center;
                    alignment_record.Right = right;
                    alignment_record.Updated_on = TimeStamp();
                    alignment_record.Updated_by = dto.End_User_ID;
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    alignment = dto.Alignment
                });
            }
            catch
            {
                return "Server Error: Update Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Text_Alignment(Selected_App_Text_Alignment dto)
        {
            try
            {

                var text_alignment_record = await _UsersDBC.Selected_App_Text_AlignmentTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                bool text_float_left = false, text_center = false, text_float_right = false;

                switch (dto.Text_alignment)
                {
                    case 0: text_float_left = true; break;
                    case 1: text_center = true; break;
                    case 2: text_float_right = true; break;
                }

                if (text_alignment_record == null)
                {
                    await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Left = text_float_left,
                        Center = text_center,
                        Right = text_float_right,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    text_alignment_record.Left = text_float_left;
                    text_alignment_record.Center = text_center;
                    text_alignment_record.Right = text_float_right;
                    text_alignment_record.Updated_by = dto.End_User_ID;
                    text_alignment_record.Updated_on = TimeStamp();
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    text_alignment = dto.Text_alignment
                });
            }
            catch
            {
                return "Server Error: Update Text Alignment Selection Failed.";
            }
        }
        public async Task<string> Update_End_User_Account_Type(Account_Types dto)
        {
            try
            {
                var account_type_record = await _UsersDBC.Account_TypeTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (account_type_record == null)
                {
                    await _UsersDBC.Account_TypeTbl.AddAsync(new Account_TypeTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Type = dto.Type,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    account_type_record.End_User_ID = dto.End_User_ID;
                    account_type_record.Type = dto.Type;
                    account_type_record.Updated_on = TimeStamp();
                    account_type_record.Created_on = TimeStamp();
                    account_type_record.Updated_by = dto.End_User_ID;
                }
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    account_type = dto.Type
                });
            }
            catch
            {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Grid_Type(Selected_App_Grid_Type dto)
        {
            try
            {
                var grid_type_record = await _UsersDBC.Selected_App_Grid_TypeTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (grid_type_record == null)
                {
                    await _UsersDBC.Selected_App_Grid_TypeTbl.AddAsync(new Selected_App_Grid_TypeTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Grid = dto.Grid,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    grid_type_record.End_User_ID = dto.End_User_ID;
                    grid_type_record.Grid = dto.Grid;
                    grid_type_record.Updated_on = TimeStamp();
                    grid_type_record.Created_on = TimeStamp();
                    grid_type_record.Updated_by = dto.End_User_ID;
                }
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    grid = dto.Grid
                });
            }
            catch
            {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Language(Selected_Language dto)
        {
            try
            {
                var language_record = await _UsersDBC.Selected_LanguageTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (language_record == null)
                {
                    await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Language_code = dto.Language,
                        Region_code = dto.Region
                    });
                }
                else
                {
                    language_record.End_User_ID = dto.End_User_ID;
                    language_record.Language_code = dto.Language;
                    language_record.Region_code = dto.Region;
                    language_record.Updated_on = TimeStamp();
                    language_record.Created_by = dto.End_User_ID;
                    language_record.Created_on = TimeStamp();
                    language_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    region = dto.Region,
                    language = dto.Language
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Nav_Lock(Selected_Navbar_Lock dto)
        {
            try
            {
                var update_existing_record_attempt = await _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Updated_on, TimeStamp())
                    .SetProperty(col => col.Locked, dto.Locked)
                );

                if (update_existing_record_attempt == 0)
                {
                    await _UsersDBC.Selected_Navbar_LockTbl.AddAsync(new Selected_Navbar_LockTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Locked = dto.Locked
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    locked = dto.Locked
                });
            }
            catch
            {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Status(Selected_Status dto)
        {
            try
            {
                var status_record = await _UsersDBC.Selected_StatusTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                bool online = false, offline = false, dnd = false, away = false, hidden = false, custom = false;

                switch (dto.Status)
                {
                    case 0: offline = true; break;
                    case 1: hidden = true; break;
                    case 2: online = true; break;
                    case 3: away = true; break;
                    case 4: dnd = true; break;
                    case 5: custom = true; break;
                    default: break;
                }

                if (status_record == null)
                {
                    await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Online = online,
                        Offline = offline,
                        Hidden = hidden,
                        DND = dnd,
                        Away = away,
                        Custom = custom,
                        Custom_lbl = dto.Custom_lbl
                    });

                }
                else
                {
                    status_record.End_User_ID = dto.End_User_ID;
                    status_record.Updated_on = TimeStamp();
                    status_record.Created_on = TimeStamp();
                    status_record.Updated_by = dto.End_User_ID;
                    status_record.Created_by = dto.End_User_ID;
                    status_record.Online = online;
                    status_record.Offline = offline;
                    status_record.Hidden = hidden;
                    status_record.DND = dnd;
                    status_record.Away = away;
                    status_record.Custom = custom;
                    status_record.Custom_lbl = dto.Custom_lbl;
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    status = dto.Status
                });
            }
            catch
            {
                return "Server Error: Update Status Selection Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Theme(Selected_Theme dto)
        {
            try
            {
                bool Light = false, Night = false, Custom = false;

                switch (dto.Theme)
                {
                    case 0: Light = true; break;
                    case 1: Night = true; break;
                    case 2: Custom = true; break;
                }

                var theme_record = await _UsersDBC.Selected_ThemeTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (theme_record == null)
                {
                    await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Light = Light,
                        Night = Night,
                        Custom = Custom,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    theme_record.End_User_ID = dto.End_User_ID;
                    theme_record.Light = Light;
                    theme_record.Night = Night;
                    theme_record.Custom = Custom;
                    theme_record.Updated_on = TimeStamp();
                    theme_record.Created_by = dto.End_User_ID;
                    theme_record.Created_on = TimeStamp();
                    theme_record.Updated_by = dto.End_User_ID;
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = dto.Theme
                });
            }
            catch
            {
                return "Server Error: Update Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Border_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_bored_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_bored_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Border_Color = dto.Card_Border_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_bored_color_record.End_User_ID = dto.End_User_ID;
                    card_bored_color_record.Card_Border_Color = dto.Card_Border_Color;
                    card_bored_color_record.Updated_on = TimeStamp();
                    card_bored_color_record.Created_by = dto.End_User_ID;
                    card_bored_color_record.Created_on = TimeStamp();
                    card_bored_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_border_color = dto.Card_Border_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Header_Font = dto.Card_Header_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Header_Font = dto.Card_Header_Font;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_font = dto.Card_Header_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_header_background_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_header_background_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Header_Background_Color = dto.Card_Header_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_header_background_color_record.End_User_ID = dto.End_User_ID;
                    card_header_background_color_record.Card_Header_Background_Color = dto.Card_Header_Background_Color;
                    card_header_background_color_record.Updated_on = TimeStamp();
                    card_header_background_color_record.Created_by = dto.End_User_ID;
                    card_header_background_color_record.Created_on = TimeStamp();
                    card_header_background_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_background_color = dto.Card_Header_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_header_font_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_header_font_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Header_Font_Color = dto.Card_Header_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_header_font_color_record.End_User_ID = dto.End_User_ID;
                    card_header_font_color_record.Card_Header_Font_Color = dto.Card_Header_Font_Color;
                    card_header_font_color_record.Updated_on = TimeStamp();
                    card_header_font_color_record.Created_by = dto.End_User_ID;
                    card_header_font_color_record.Created_on = TimeStamp();
                    card_header_font_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_font_color = dto.Card_Header_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_footer_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_footer_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Footer_Font = dto.Card_Footer_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_footer_font_record.End_User_ID = dto.End_User_ID;
                    card_footer_font_record.Card_Footer_Font = dto.Card_Footer_Font;
                    card_footer_font_record.Updated_on = TimeStamp();
                    card_footer_font_record.Created_by = dto.End_User_ID;
                    card_footer_font_record.Created_on = TimeStamp();
                    card_footer_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_font = dto.Card_Footer_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_footer_background_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_footer_background_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Footer_Background_Color = dto.Card_Footer_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_footer_background_color_record.End_User_ID = dto.End_User_ID;
                    card_footer_background_color_record.Card_Footer_Background_Color = dto.Card_Footer_Background_Color;
                    card_footer_background_color_record.Updated_on = TimeStamp();
                    card_footer_background_color_record.Created_by = dto.End_User_ID;
                    card_footer_background_color_record.Created_on = TimeStamp();
                    card_footer_background_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_background_color = dto.Card_Footer_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }

        public async Task<string> Update_End_User_Card_Footer_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_footer_font_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_footer_font_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Footer_Font_Color = dto.Card_Footer_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_footer_font_color_record.End_User_ID = dto.End_User_ID;
                    card_footer_font_color_record.Card_Footer_Font_Color = dto.Card_Footer_Font_Color;
                    card_footer_font_color_record.Updated_on = TimeStamp();
                    card_footer_font_color_record.Created_by = dto.End_User_ID;
                    card_footer_font_color_record.Created_on = TimeStamp();
                    card_footer_font_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_font_color = dto.Card_Footer_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Body_Font = dto.Card_Body_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Body_Font = dto.Card_Body_Font;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_font = dto.Card_Body_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Body_Background_Color = dto.Card_Body_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Body_Background_Color = dto.Card_Body_Background_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_background_color = dto.Card_Body_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Body_Font_Color = dto.Card_Body_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Body_Font_Color = dto.Card_Body_Font_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_font_color = dto.Card_Body_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Navigation_Menu_Font = dto.Navigation_Menu_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Navigation_Menu_Font = dto.Navigation_Menu_Font;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_font = dto.Navigation_Menu_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Navigation_Menu_Background_Color = dto.Navigation_Menu_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Navigation_Menu_Background_Color = dto.Navigation_Menu_Background_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_Background_color = dto.Navigation_Menu_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Navigation_Menu_Font_Color = dto.Navigation_Menu_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Navigation_Menu_Font_Color = dto.Navigation_Menu_Font_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_font_color = dto.Navigation_Menu_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Button_Font = dto.Button_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_font_record.End_User_ID = dto.End_User_ID;
                    button_font_record.Button_Font = dto.Button_Font;
                    button_font_record.Updated_on = TimeStamp();
                    button_font_record.Created_by = dto.End_User_ID;
                    button_font_record.Created_on = TimeStamp();
                    button_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_font = dto.Button_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_background_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_background_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Button_Background_Color = dto.Button_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_background_color_record.End_User_ID = dto.End_User_ID;
                    button_background_color_record.Button_Background_Color = dto.Button_Background_Color;
                    button_background_color_record.Updated_on = TimeStamp();
                    button_background_color_record.Created_by = dto.End_User_ID;
                    button_background_color_record.Created_on = TimeStamp();
                    button_background_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_background_color = dto.Button_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Button_Font_Color = dto.Button_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_font_record.End_User_ID = dto.End_User_ID;
                    button_font_record.Button_Font_Color = dto.Button_Font_Color;
                    button_font_record.Updated_on = TimeStamp();
                    button_font_record.Created_by = dto.End_User_ID;
                    button_font_record.Created_on = TimeStamp();
                    button_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_font_color = dto.Button_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Password(Password_Change dto)
        {
            try
            {
                if (!dto.Email_address.IsNullOrEmpty())
                {
                    await _UsersDBC.Login_PasswordTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Password, Password.Create_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes($"{dto.New_password}"), Encoding.UTF8.GetBytes($"{dto.Email_address}{_Constants.JWT_SECURITY_KEY}")))
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                        .SetProperty(col => col.Updated_on, TimeStamp())
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return "Update End User Password Completed.";
            }
            catch
            {
                return "Server Error: Update Password Failed.";
            }
        }
        public async Task<string> Update_End_User_Login_Time_Stamp(Login_Time_Stamp dto)
        {
            try
            {
                var login_record = await _UsersDBC.Login_Time_StampTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (login_record == null)
                {
                    await _UsersDBC.Login_Time_StampTbl.AddAsync(new Login_Time_StampTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Deleted = false,
                        Deleted_on = 0,
                        Deleted_by = 0,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Login_on = TimeStamp(),
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Remote_IP = dto.Remote_IP,
                        Remote_Port = dto.Remote_Port,
                        Server_IP = dto.Server_IP,
                        Server_Port = dto.Server_Port,
                        Client_IP = dto.Client_IP,
                        Client_Port = dto.Client_Port,
                        User_agent = dto.User_agent,
                        Down_link = dto.Down_link,
                        Connection_type = dto.Connection_type,
                        RTT = dto.RTT,
                        Data_saver = dto.Data_saver,
                        Device_ram_gb = dto.Device_ram_gb,
                        Orientation = dto.Orientation,
                        Screen_width = dto.Screen_width,
                        Screen_height = dto.Screen_height,
                        Window_height = dto.Window_height,
                        Window_width = dto.Window_width,
                        Color_depth = dto.Color_depth,
                        Pixel_depth = dto.Pixel_depth
                    });
                }
                else
                {
                    login_record.End_User_ID = dto.End_User_ID;
                    login_record.Updated_on = TimeStamp();
                    login_record.Created_on = TimeStamp();
                    login_record.Updated_by = dto.End_User_ID;
                    login_record.Created_by = dto.End_User_ID;
                    login_record.Login_on = TimeStamp();
                    login_record.Location = dto.Location;
                    login_record.Client_time = dto.Client_time;
                    login_record.Remote_IP = dto.Remote_IP;
                    login_record.Remote_Port = dto.Remote_Port;
                    login_record.Server_IP = dto.Server_IP;
                    login_record.Server_Port = dto.Server_Port;
                    login_record.Client_IP = dto.Client_IP;
                    login_record.Client_Port = dto.Client_Port;
                    login_record.User_agent = dto.User_agent;
                    login_record.Down_link = dto.Down_link;
                    login_record.Connection_type = dto.Connection_type;
                    login_record.RTT = dto.RTT;
                    login_record.Data_saver = dto.Data_saver;
                    login_record.Device_ram_gb = dto.Device_ram_gb;
                    login_record.Orientation = dto.Orientation;
                    login_record.Screen_width = dto.Screen_width;
                    login_record.Screen_height = dto.Screen_height;
                    login_record.Window_height = dto.Window_height;
                    login_record.Window_width = dto.Window_width;
                    login_record.Color_depth = dto.Color_depth;
                    login_record.Pixel_depth = dto.Pixel_depth;
                }

                return Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    login_on = TimeStamp()
                })).Result;
            }
            catch
            {
                return "Server Error: Update Login Failed.";
            }
        }
        public async Task<string> Update_End_User_Logout(Logout_Time_Stamp dto)
        {
            var logout_time_stamp_record = await _UsersDBC.Logout_Time_StampTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (logout_time_stamp_record == null)
            {
                await _UsersDBC.Logout_Time_StampTbl.AddAsync(new Logout_Time_StampTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Logout_on = TimeStamp(),
                    Updated_by = dto.End_User_ID,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                await _UsersDBC.SaveChangesAsync();
            }
            else
            {
                logout_time_stamp_record.End_User_ID = dto.End_User_ID;
                logout_time_stamp_record.Logout_on = TimeStamp();
                logout_time_stamp_record.Updated_by = dto.End_User_ID;
                logout_time_stamp_record.Updated_on = TimeStamp();
                logout_time_stamp_record.Created_on = TimeStamp();
                logout_time_stamp_record.Location = dto.Location;
                logout_time_stamp_record.Remote_IP = dto.Remote_IP;
                logout_time_stamp_record.Remote_Port = dto.Remote_Port;
                logout_time_stamp_record.Server_IP = dto.Server_IP;
                logout_time_stamp_record.Server_Port = dto.Server_Port;
                logout_time_stamp_record.Client_IP = dto.Client_IP;
                logout_time_stamp_record.Client_Port = dto.Client_Port;
                logout_time_stamp_record.Client_time = dto.Client_Time_Parsed;
                logout_time_stamp_record.User_agent = dto.User_agent;
                logout_time_stamp_record.Window_height = dto.Window_height;
                logout_time_stamp_record.Window_width = dto.Window_width;
                logout_time_stamp_record.Screen_height = dto.Screen_height;
                logout_time_stamp_record.Screen_width = dto.Screen_width;
                logout_time_stamp_record.RTT = dto.RTT;
                logout_time_stamp_record.Orientation = dto.Orientation;
                logout_time_stamp_record.Data_saver = dto.Data_saver;
                logout_time_stamp_record.Color_depth = dto.Color_depth;
                logout_time_stamp_record.Pixel_depth = dto.Pixel_depth;
                logout_time_stamp_record.Connection_type = dto.Connection_type;
                logout_time_stamp_record.Down_link = dto.Down_link;
                logout_time_stamp_record.Device_ram_gb = dto.Device_ram_gb;
            }

            return Task.FromResult(JsonSerializer.Serialize(new
            {
                End_User_ID = dto.End_User_ID,
                logout_on = TimeStamp()
            })).Result;
        }
        public async Task<string> Update_End_User_First_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    First_Name = dto.First_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.First_Name = dto.First_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                first_name = dto.First_name
            });
        }
        public async Task<string> Update_End_User_Last_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Last_Name = dto.Last_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Last_Name = dto.Last_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                last_name = dto.Last_name
            });
        }
        public async Task<string> Update_End_User_Middle_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Middle_Name = dto.Middle_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Middle_Name = dto.Middle_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                middle_name = dto.Middle_name
            });
        }
        public async Task<string> Update_End_User_Maiden_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Maiden_Name = dto.Maiden_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Maiden_Name = dto.Maiden_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                maiden_name = dto.Maiden_name
            });
        }
        public async Task<string> Update_End_User_Gender(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Gender = dto.Gender,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Gender = dto.Gender;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                gender = dto.Gender
            });
        }
        public async Task<string> Update_End_User_Ethnicity(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Ethnicity = dto.Ethnicity,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Ethnicity = dto.Ethnicity;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                ethnicity = dto.Ethnicity
            });
        }
        public async Task<string> Update_End_User_Birth_Date(Identities dto)
        {
            var birth_date_record = await _UsersDBC.Birth_DateTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (birth_date_record == null)
            {
                await _UsersDBC.Birth_DateTbl.AddAsync(new Birth_DateTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Month = dto.Month,
                    Day = dto.Day,
                    Year = dto.Year,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                birth_date_record.End_User_ID = dto.End_User_ID;
                birth_date_record.Day = dto.Day;
                birth_date_record.Month = dto.Month;
                birth_date_record.Year = dto.Year;
                birth_date_record.Updated_on = TimeStamp();
                birth_date_record.Created_on = TimeStamp();
                birth_date_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                birth_month = dto.Month,
                birth_day = dto.Day,
                birth_year = dto.Year,
            });
        }
        public async Task<string> Update_End_User_Account_Groups(Account_Group dto)
        {
            try
            {
                var account_group_record = await _UsersDBC.Account_GroupsTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (account_group_record == null)
                {
                    await _UsersDBC.Account_GroupsTbl.AddAsync(new Account_GroupsTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Groups = dto.Groups,
                        Updated_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });
                }
                else
                {
                    account_group_record.End_User_ID = dto.End_User_ID;
                    account_group_record.Groups = dto.Groups;
                    account_group_record.Updated_on = TimeStamp();
                    account_group_record.Updated_by = dto.End_User_ID;
                    account_group_record.Created_on = TimeStamp();
                }
                return JsonSerializer.Serialize(new
                {
                    end_user_groups = dto.Groups
                });
            }
            catch
            {
                return "Server Error: Update Account Groups Failed.";
            }
        }
        public async Task<string> Update_End_User_Account_Roles(Account_Role dto)
        {
            try
            {
                var account_roles_record = await _UsersDBC.Account_RolesTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (account_roles_record == null)
                {
                    await _UsersDBC.Account_RolesTbl.AddAsync(new Account_RolesTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Roles = dto.Roles,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Updated_by = dto.End_User_ID,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });
                }
                else
                {
                    account_roles_record.End_User_ID = dto.End_User_ID;
                    account_roles_record.Roles = dto.Roles;
                    account_roles_record.Updated_by = dto.End_User_ID;
                    account_roles_record.Updated_on = TimeStamp();
                }

                return JsonSerializer.Serialize(new
                {
                    roles = dto.Roles
                });
            }
            catch
            {
                return "Server Error: Update End User Roles Failed.";
            }
        }

    }
}
