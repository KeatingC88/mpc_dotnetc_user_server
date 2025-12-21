using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Security.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository
{
    public class Users_Repository_Read : IUsers_Repository_Read
    {
        private long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private readonly Users_Database_Context _UsersDBC;
        private readonly Constants _Constants;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly IPassword Password;
        private readonly IUsers_Repository_Update IUsers_Repository_Update;

        public Users_Repository_Read(
            Users_Database_Context Users_Database_Context,
            Constants constants,
            IAES aes,
            IJWT jwt,
            IPassword password,
            IUsers_Repository_Update iuser_repository_update
        )
        {
            _UsersDBC = Users_Database_Context;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Password = password;
            IUsers_Repository_Update = iuser_repository_update;
        }

        public Task<bool> Read_ID_Exists_In_Users_ID(long user_id)
        {
            return Task.FromResult(_UsersDBC.User_IDsTbl.Any(x => x.ID == user_id && x.Deleted == false));
        }
        public Task<bool> Read_ID_Exists_In_Twitch_IDs(long user_id)
        {
            return Task.FromResult(_UsersDBC.Twitch_IDsTbl.Any(x => x.Twitch_ID == user_id && x.Deleted == false));
        }
        public Task<bool> Read_ID_Exists_In_Discord_IDs(long user_id)
        {
            return Task.FromResult(_UsersDBC.Discord_IDsTbl.Any(x => x.Discord_ID == user_id && x.Deleted == false));
        }

        public async Task<long> Read_User_ID_By_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.Email_Address == email_address).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<string?> Read_User_Email_By_ID(long id)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.End_User_ID == id).Select(x => x.Email_Address).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Twitch_Account_ID(long twitch_id)
        {
            return await Task.FromResult(_UsersDBC.Twitch_IDsTbl.Where(x => x.Twitch_ID == twitch_id).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Twitch_Account_Email(string twitch_email)
        {
            return await Task.FromResult(_UsersDBC.Twitch_Email_AddressTbl.Where(x => x.Email_Address == twitch_email.ToUpper()).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Discord_Account_ID(long discord_id)
        {
            return await Task.FromResult(_UsersDBC.Discord_IDsTbl.Where(x => x.Discord_ID == discord_id).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Discord_Account_Email(string discord_email)
        {
            return await Task.FromResult(_UsersDBC.Discord_Email_AddressTbl.Where(x => x.Email_Address == discord_email).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<User_Data_DTO> Read_User_Data_By_ID(long end_user_id)
        {
            var user = (from end_user in _UsersDBC.User_IDsTbl

            join account_type_table in _UsersDBC.Account_TypeTbl
                on end_user.ID equals account_type_table.End_User_ID into resulting_account_type_table
            from account_type_table in resulting_account_type_table.DefaultIfEmpty()

            join login_email_table in _UsersDBC.Login_Email_AddressTbl
                on end_user.ID equals login_email_table.End_User_ID into resulting_email_table
            from login_email_table in resulting_email_table.DefaultIfEmpty()

            join selected_language_table in _UsersDBC.Selected_LanguageTbl
                on end_user.ID equals selected_language_table.End_User_ID into resulting_selected_language_table
            from selected_language_table in resulting_selected_language_table.DefaultIfEmpty()

            join selected_name_table in _UsersDBC.Selected_NameTbl
                on end_user.ID equals selected_name_table.End_User_ID into resulting_name_table
            from selected_name_table in resulting_name_table.DefaultIfEmpty()

            join selected_custom_design_table in _UsersDBC.Selected_App_Custom_DesignTbl
                on end_user.ID equals selected_custom_design_table.End_User_ID into resulting_selected_custom_design_table
            from selected_custom_design_table in resulting_selected_custom_design_table.DefaultIfEmpty()

            join account_groups_table in _UsersDBC.Account_GroupsTbl
                on end_user.ID equals account_groups_table.End_User_ID into resulting_account_groups_table
            from account_groups_table in resulting_account_groups_table.DefaultIfEmpty()

            join account_roles_table in _UsersDBC.Account_RolesTbl
                on end_user.ID equals account_roles_table.End_User_ID into resulting_account_roles_table
            from account_roles_table in resulting_account_roles_table.DefaultIfEmpty()

            join selected_grid_table in _UsersDBC.Selected_App_Grid_TypeTbl
                on end_user.ID equals selected_grid_table.End_User_ID into resulting_selected_grid_table
            from selected_grid_table in resulting_selected_grid_table.DefaultIfEmpty()

            join selected_navbar_lock_table in _UsersDBC.Selected_Navbar_LockTbl
                on end_user.ID equals selected_navbar_lock_table.End_User_ID into resulting_selected_navbar_lock_table
            from selected_navbar_lock_table in resulting_selected_navbar_lock_table.DefaultIfEmpty()

            join status_table in _UsersDBC.Selected_StatusTbl
                on end_user.ID equals status_table.End_User_ID into resulting_status_table
            from status_table in resulting_status_table.DefaultIfEmpty()

            join avatar_table in _UsersDBC.Selected_AvatarTbl
                on end_user.ID equals avatar_table.End_User_ID into resulting_avatar_table
            from avatar_table in resulting_avatar_table.DefaultIfEmpty()

            join theme_table in _UsersDBC.Selected_ThemeTbl
                on end_user.ID equals theme_table.End_User_ID into resulting_theme_table
            from theme_table in resulting_theme_table.DefaultIfEmpty()

            join alignment_table in _UsersDBC.Selected_App_AlignmentTbl
                on end_user.ID equals alignment_table.End_User_ID into resulting_alignment_table
            from alignment_table in resulting_alignment_table.DefaultIfEmpty()

            join text_alignment_table in _UsersDBC.Selected_App_Text_AlignmentTbl
                on end_user.ID equals text_alignment_table.End_User_ID into resulting_text_alignment_table
            from text_alignment_table in resulting_text_alignment_table.DefaultIfEmpty()

            join identity_table in _UsersDBC.IdentityTbl
                on end_user.ID equals identity_table.End_User_ID into resulting_identity_table
            from identity_table in resulting_identity_table.DefaultIfEmpty()

            join birth_date_table in _UsersDBC.Birth_DateTbl
                on end_user.ID equals birth_date_table.End_User_ID into resulting_birth_date_table
            from birth_date_table in resulting_birth_date_table.DefaultIfEmpty()

            join login_table in _UsersDBC.Login_Time_StampTbl
                on end_user.ID equals login_table.End_User_ID into resulting_login_table
            from login_table in resulting_login_table.DefaultIfEmpty()

            join twitch_id_table in _UsersDBC.Twitch_IDsTbl
                on end_user.ID equals twitch_id_table.End_User_ID into resulting_twitch_table
            from twitch_id_table in resulting_twitch_table.DefaultIfEmpty()

            join twitch_email_address_table in _UsersDBC.Twitch_Email_AddressTbl
                on end_user.ID equals twitch_email_address_table.End_User_ID into resulting_twitch_email_address_table
            from twitch_email_address_table in resulting_twitch_email_address_table.DefaultIfEmpty()

            join logout_table in _UsersDBC.Logout_Time_StampTbl
                on end_user.ID equals logout_table.End_User_ID into resulting_logout_table
            from logout_table in resulting_logout_table.DefaultIfEmpty()

            where end_user.ID == end_user_id

            select new
            {
                end_user_id = end_user.ID,
                created_on = end_user.Created_on,
                account_type = account_type_table.Type,
                email_address = login_email_table.Email_Address,
                name = selected_name_table.Name,
                public_id = end_user.Public_ID,
                current_language = $@"{selected_language_table.Language_code}-{selected_language_table.Region_code}",
                language = selected_language_table.Language_code,
                region = selected_language_table.Region_code,
                groups = account_groups_table.Groups,
                roles = account_roles_table.Roles,
                grid_type = selected_grid_table.Grid,
                nav_lock = selected_navbar_lock_table.Locked,
                online_status = status_table.Online,
                offline_status = status_table.Offline,
                hidden_status = status_table.Hidden,
                away_status = status_table.Away,
                dnd_status = status_table.DND,
                custom_status = status_table.Custom,
                custom_lbl = status_table.Custom_lbl,
                avatar_url_path = avatar_table.Avatar_url_path,
                avatar_title = avatar_table.Avatar_title,
                light_theme = theme_table.Light,
                night_theme = theme_table.Night,
                custom_theme = theme_table.Custom,
                left_alignment = alignment_table.Left,
                center_alignment = alignment_table.Center,
                right_alignment = alignment_table.Right,
                left_text_alignment = text_alignment_table.Left,
                center_text_alignment = text_alignment_table.Center,
                right_text_alignment = text_alignment_table.Right,
                login_on = login_table.Login_on,
                logout_on = logout_table != null ? logout_table.Logout_on : (long?)null,
                gender = identity_table.Gender,
                birth_day = birth_date_table.Day,
                birth_month = birth_date_table.Month,
                birth_year = birth_date_table.Year,
                first_name = identity_table.First_Name,
                last_name = identity_table.Last_Name,
                middle_name = identity_table.Middle_Name,
                maiden_name = identity_table.Maiden_Name,
                ethnicity = identity_table.Ethnicity,
                twitch_user_id = twitch_id_table != null ? twitch_id_table.Twitch_ID : (long?)null,
                twitch_user_name = twitch_id_table != null ? twitch_id_table.User_Name : null,
                twitch_email_address = twitch_email_address_table != null ? twitch_email_address_table.Email_Address : null,
                card_border_color = selected_custom_design_table.Card_Border_Color,
                card_header_font = selected_custom_design_table.Card_Header_Font,
                card_header_font_color = selected_custom_design_table.Card_Header_Font_Color,
                card_header_background_color = selected_custom_design_table.Card_Header_Background_Color,
                card_body_font = selected_custom_design_table.Card_Body_Font,
                card_body_background_color = selected_custom_design_table.Card_Body_Background_Color,
                card_body_font_color = selected_custom_design_table.Card_Body_Font_Color,
                card_footer_font_color = selected_custom_design_table.Card_Footer_Font_Color,
                card_footer_font = selected_custom_design_table.Card_Footer_Font,
                card_footer_background_color = selected_custom_design_table.Card_Footer_Background_Color,
                navigation_menu_background_color = selected_custom_design_table.Navigation_Menu_Background_Color,
                navigation_menu_font_color = selected_custom_design_table.Navigation_Menu_Font_Color,
                navigation_menu_font = selected_custom_design_table.Navigation_Menu_Font,
                button_background_color = selected_custom_design_table.Button_Background_Color,
                button_font = selected_custom_design_table.Button_Font,
                button_font_color = selected_custom_design_table.Button_Font_Color
            }).FirstOrDefault();

            if (user == null)
                return await Task.FromResult(new User_Data_DTO { });

            await _UsersDBC.SaveChangesAsync();

            return await Task.FromResult(new User_Data_DTO
            {
                id = user.end_user_id,
                account_type = user.account_type,
                email_address = user.email_address,
                name = $@"{user.name}#{user.public_id}",
                current_language = user.current_language,
                language = user.language,
                region = user.region,
                groups = user.groups ?? "",
                roles = user.roles ?? "",
                grid_type = user.grid_type,
                nav_lock = user.nav_lock,
                custom_lbl = user.custom_lbl ?? "",
                created_on = user.created_on,
                avatar_url_path = user.avatar_url_path ?? "",
                avatar_title = user.avatar_title ?? "",
                twitch_user_name = user.twitch_user_name,
                twitch_id = user.twitch_user_id ?? 0,
                login_on = user.login_on,
                logout_on = user.logout_on ?? 0,

                online_status =
                    user.online_status ? (byte)2 :
                    user.offline_status ? (byte)0 :
                    user.hidden_status ? (byte)1 :
                    user.away_status ? (byte)3 :
                    user.dnd_status ? (byte)4 :
                    user.custom_status ? (byte)5 : (byte)0,

                theme =
                    user.light_theme ? (byte)0 :
                    user.night_theme ? (byte)1 :
                    user.custom_theme ? (byte)2 : (byte)0,

                alignment =
                    user.left_alignment ? (byte)0 :
                    user.center_alignment ? (byte)1 :
                    user.right_alignment ? (byte)2 : (byte)0,

                text_alignment =
                    user.left_text_alignment ? (byte)0 :
                    user.center_text_alignment ? (byte)1 :
                    user.right_text_alignment ? (byte)2 : (byte)0,

                gender = user.gender ?? (byte)2,
                birth_day = user.birth_day ?? 0,
                birth_month = user.birth_month ?? 0,
                birth_year = user.birth_year ?? 0,

                first_name = user.first_name ?? "",
                last_name = user.last_name ?? "",
                middle_name = user.middle_name ?? "",
                maiden_name = user.maiden_name ?? "",
                ethnicity = user.ethnicity ?? "",

                card_border_color = user.card_border_color,
                card_header_font = user.card_header_font,
                card_header_font_color = user.card_header_font_color,
                card_header_background_color = user.card_header_background_color,
                card_body_font = user.card_body_font,
                card_body_background_color = user.card_body_background_color,
                card_body_font_color = user.card_body_font_color,
                card_footer_font_color = user.card_footer_font_color,
                card_footer_font = user.card_footer_font,
                card_footer_background_color = user.card_footer_background_color,
                navigation_menu_background_color = user.navigation_menu_background_color,
                navigation_menu_font_color = user.navigation_menu_font_color,
                navigation_menu_font = user.navigation_menu_font,
                button_background_color = user.button_background_color,
                button_font = user.button_font,
                button_font_color = user.button_font_color
            });
        }
        public async Task<byte[]?> Read_User_Password_Hash_By_ID(long user_id)
        {
            return await Task.FromResult(_UsersDBC.Login_PasswordTbl.Where(user => user.End_User_ID == user_id).Select(user => user.Password).SingleOrDefault());
        }
        public async Task<User_Token_Data_DTO> Read_Require_Token_Data_By_ID(long end_user_id)
        {
            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? twitch_email_address = _UsersDBC.Twitch_Email_AddressTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? discord_email_address = _UsersDBC.Discord_Email_AddressTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            long? twitch_id = _UsersDBC.Twitch_IDsTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.End_User_ID).SingleOrDefault();
            long? discord_id = _UsersDBC.Discord_IDsTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.End_User_ID).SingleOrDefault();
            string? groups = _UsersDBC.Account_GroupsTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Groups).SingleOrDefault();
            string? roles = _UsersDBC.Account_RolesTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Roles).SingleOrDefault();
            byte account_type = _UsersDBC.Account_TypeTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Type).SingleOrDefault();

            return await Task.FromResult(new User_Token_Data_DTO
            {
                id = end_user_id,
                account_type = account_type,
                email_address = email_address ?? "",
                twitch_id = twitch_id,
                twitch_email_address = twitch_email_address ?? "",
                discord_id = discord_id,
                discord_email_address = discord_email_address ?? "",
                groups = groups ?? "",
                roles = roles ?? ""
            });
        }
        public async Task<string> Read_User_Profile_By_ID(long user_id)
        {
            var userProfile = (from end_user in _UsersDBC.User_IDsTbl

                               join login_email_table in _UsersDBC.Login_Email_AddressTbl
                                   on end_user.ID equals login_email_table.End_User_ID into resulting_email_table
                               from login_email_table in resulting_email_table.DefaultIfEmpty()

                               join selected_language_table in _UsersDBC.Selected_LanguageTbl
                                   on end_user.ID equals selected_language_table.End_User_ID into resulting_selected_language_table
                               from selected_language_table in resulting_selected_language_table.DefaultIfEmpty()

                               join avatar_table in _UsersDBC.Selected_AvatarTbl
                                   on end_user.ID equals avatar_table.End_User_ID into resulting_avatar_table
                               from avatar_table in resulting_avatar_table.DefaultIfEmpty()

                               join selected_name_table in _UsersDBC.Selected_NameTbl
                                   on end_user.ID equals selected_name_table.End_User_ID into resulting_name_table
                               from selected_name_table in resulting_name_table.DefaultIfEmpty()

                               join login_table in _UsersDBC.Login_Time_StampTbl
                                   on end_user.ID equals login_table.End_User_ID into resulting_login_table
                               from login_table in resulting_login_table.DefaultIfEmpty()

                               join logout_table in _UsersDBC.Logout_Time_StampTbl
                                   on end_user.ID equals logout_table.End_User_ID into resulting_logout_table
                               from logout_table in resulting_logout_table.DefaultIfEmpty()

                               join status_table in _UsersDBC.Selected_StatusTbl
                                   on end_user.ID equals status_table.End_User_ID into resulting_status_table
                               from status_table in resulting_status_table.DefaultIfEmpty()

                               where end_user.ID == user_id

                               select new
                               {
                                   email_address = login_email_table.Email_Address,
                                   name = selected_name_table.Name,
                                   login_on = login_table.Login_on,
                                   logout_on = logout_table != null ? logout_table.Logout_on : 0,
                                   language = selected_language_table != null
                                       ? @$"{selected_language_table.Language_code}-{selected_language_table.Region_code}"
                                       : null,
                                   language_code = selected_language_table.Language_code,
                                   region_code = selected_language_table.Region_code,
                                   avatar_url_path = avatar_table.Avatar_url_path,
                                   avatar_title = avatar_table.Avatar_title,
                                   created_on = end_user.Created_on,
                                   status_online = status_table.Online,
                                   status_offline = status_table.Offline,
                                   status_hidden = status_table.Hidden,
                                   status_away = status_table.Away,
                                   status_dnd = status_table.DND,
                                   status_custom = status_table.Custom,
                                   custom_lbl = status_table.Custom_lbl
                               }).FirstOrDefault();

            if (userProfile == null)
            {
                return await Task.FromResult(JsonSerializer.Serialize(new { }));
            }

            // Set status_code based on available flags
            byte status_code = 0;
            if (userProfile.status_offline)
                status_code = 0;
            if (userProfile.status_hidden)
                status_code = 1;
            if (userProfile.status_online)
                status_code = 2;
            if (userProfile.status_away)
                status_code = 3;
            if (userProfile.status_dnd)
                status_code = 4;
            if (userProfile.status_custom)
                status_code = 5;

            return await Task.FromResult(JsonSerializer.Serialize(new
            {
                id = user_id,
                email_address = userProfile.email_address,
                name = userProfile.name,
                login_on = userProfile.login_on,
                logout_on = userProfile.logout_on,
                language = userProfile.language,
                online_status = status_code,
                custom_lbl = userProfile.custom_lbl,
                created_on = userProfile.created_on,
                avatar_url_path = userProfile.avatar_url_path,
                avatar_title = userProfile.avatar_title
            }));
        }
        public async Task<string> Read_WebSocket_Permission_Record_For_Both_End_Users(WebSocket_Chat_Permission dto)
        {
            bool requested = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Requested).SingleOrDefault();
            bool approved = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Approved).SingleOrDefault();
            bool blocked = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Blocked).SingleOrDefault();
            bool deleted = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Deleted).SingleOrDefault();

            bool requested_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Requested).SingleOrDefault();
            bool approved_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Approved).SingleOrDefault();
            bool blocked_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Blocked).SingleOrDefault();
            bool deleted_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Deleted).SingleOrDefault();


            if (requested == true || requested_swap_ids == true)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            if (approved == true || approved_swap_ids == true)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 0,
                    blocked = 0,
                    approved = true,
                });
            }

            if (blocked == true || blocked_swap_ids == true)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 0,
                    blocked = true,
                    approved = 0,
                });
            }

            if (requested == false && approved == false && blocked == false && requested_swap_ids == false && approved_swap_ids == false && blocked_swap_ids == false && deleted == false && deleted_swap_ids == false)
            {
                await IUsers_Repository_Update.Update_Chat_Web_Socket_Permissions(dto);
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            if (requested == false && approved == false && blocked == false && requested_swap_ids == false && approved_swap_ids == false && blocked_swap_ids == false && (deleted == true || deleted_swap_ids == true))
            {
                await IUsers_Repository_Update.Update_Chat_Web_Socket_Permissions(new WebSocket_Chat_Permission
                {
                    End_User_ID = dto.End_User_ID,
                    Participant_ID = dto.Participant_ID,
                    Requested = true,
                    Blocked = false,
                    Approved = false
                });
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            return JsonSerializer.Serialize(new
            {
                requested = 0,
                blocked = 0,
                approved = 0,
            });
        }
        public async Task<string> Read_End_User_WebSocket_Sent_Chat_Requests(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.End_User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == user_id && x.Requested == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Sent_Chat_Blocks(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.End_User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == user_id && x.Blocked == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Sent_Chat_Approvals(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.End_User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == user_id && x.Approved == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Received_Chat_Requests(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Requested == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_Received_Friend_Requests(long user_id)
        {
            if (!_UsersDBC.Friends_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.Friends_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Requested == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Received_Chat_Blocks(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Blocked == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Received_Chat_Approvals(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Approved == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_Friend_Permissions_By_ID(long user_id)
        {
            var user_sent_permissions = _UsersDBC.Friends_PermissionTbl
                .Where(friend_permission => friend_permission.End_User_ID == user_id && friend_permission.Deleted == false)
                .ToDictionary(friend_permission => friend_permission.Participant_ID,
                    friend_permission => new
                    {
                        request = friend_permission.Requested,
                        block = friend_permission.Blocked,
                        approve = friend_permission.Approved,
                        record_updated_by = friend_permission.Updated_by,
                        record_updated_on = friend_permission.Updated_on,
                        record_created_on = friend_permission.Created_on,
                        record_created_by = friend_permission.Created_by,
                        time_stamp = TimeStamp()
                    }
                );

            var user_received_permissions = _UsersDBC.Friends_PermissionTbl
                .Where(friend_permission => friend_permission.Participant_ID == user_id && friend_permission.Deleted == false)
                .ToDictionary(friend_permission => friend_permission.End_User_ID,
                    friend_permission => new {
                        request = friend_permission.Requested,
                        block = friend_permission.Blocked,
                        approve = friend_permission.Approved,
                        record_updated_by = friend_permission.Updated_by,
                        record_updated_on = friend_permission.Updated_on,
                        record_created_on = friend_permission.Created_on,
                        record_created_by = friend_permission.Created_by,
                        time_stamp = TimeStamp()
                    }
                );

            return await Task.FromResult(JsonSerializer.Serialize(new
            {
                sent_permissions = user_sent_permissions,
                received_permissions = user_received_permissions,
                time_stamped = TimeStamp()
            }));
        }
        public async Task<byte> Read_End_User_Selected_Status(long user_id)
        {
            var status = await _UsersDBC.Selected_StatusTbl.Where(x => x.End_User_ID == user_id).Select(x => new {
                x.Online,
                x.Offline,
                x.Hidden,
                x.Away,
                x.DND,
                x.Custom
            }).SingleOrDefaultAsync();

            if (status == null)
                return 255;

            return status switch
            {
                var record_is when record_is.Offline == true => 0,
                var record_is when record_is.Hidden == true => 1,
                var record_is when record_is.Online == true => 2,
                var record_is when record_is.Away == true => 3,
                var record_is when record_is.DND == true => 4,
                var record_is when record_is.Custom == true => 5,
                _ => 255
            };
        }
        public async Task<bool> Read_Confirmation_Code_Exists_In_Pending_Email_Address_Registration(string Code)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Code == Code));
        }

        public async Task<bool> Read_Email_Exists_In_Login_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }

        public async Task<bool> Read_Email_Exists_In_Twitch_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Twitch_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }

        public async Task<bool> Read_Email_Exists_In_Discord_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Discord_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }

        public async Task<bool> Read_Email_Exists_In_Pending_Email_Registration(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Email_Address.ToUpper() == email_address));
        }
    }
}
