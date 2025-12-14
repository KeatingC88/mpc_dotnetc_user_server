using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Users.Selected.Deactivate;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository
{
    public class Users_Repository_Delete : IUsers_Repository_Delete
    {
        private long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private readonly Users_Database_Context _UsersDBC;
        private readonly Constants _Constants;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly IPassword Password;

        public Users_Repository_Delete(
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

        public async Task<string> Delete_Account_By_User_id(Delete_User dto)
        {
            await _UsersDBC.User_IDsTbl.Where(x => x.ID == long.Parse(dto.Target_User)).ExecuteUpdateAsync(s => s
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted, true)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_by, long.Parse(dto.ID))
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_on, TimeStamp())
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_on, TimeStamp())
                .SetProperty(User_IDsTbl => User_IDsTbl.Created_on, TimeStamp())
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_by, long.Parse(dto.ID))
            );
            await _UsersDBC.SaveChangesAsync();
            return JsonSerializer.Serialize(new
            {
                deleted_by = dto.ID,
                target_user = dto.Target_User,
            });
        }
        public async Task<string> Delete_End_User_Selected_App_Custom_Design(Selected_App_Custom_Design dto)
        {
            try
            {
                await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Card_Header_Font_Color, "")
                    .SetProperty(col => col.Card_Header_Font, "")
                    .SetProperty(col => col.Card_Header_Background_Color, "")
                    .SetProperty(col => col.Card_Body_Background_Color, "")
                    .SetProperty(col => col.Card_Body_Font, "")
                    .SetProperty(col => col.Card_Body_Font_Color, "")
                    .SetProperty(col => col.Card_Footer_Background_Color, "")
                    .SetProperty(col => col.Card_Footer_Font, "")
                    .SetProperty(col => col.Card_Footer_Font_Color, "")
                    .SetProperty(col => col.Navigation_Menu_Background_Color, "")
                    .SetProperty(col => col.Navigation_Menu_Font_Color, "")
                    .SetProperty(col => col.Navigation_Menu_Font, "")
                    .SetProperty(col => col.Button_Background_Color, "")
                    .SetProperty(col => col.Button_Font, "")
                    .SetProperty(col => col.Button_Font_Color, "")
                    .SetProperty(col => col.Card_Border_Color, "")
                    .SetProperty(col => col.Updated_on, TimeStamp())
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = 0,
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Delete_From_Web_Socket_Chat_Permissions(WebSocket_Chat_Permission dto)
        {
            try
            {
                await _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(dto => dto.Requested, dto.Requested)
                    .SetProperty(dto => dto.Blocked, dto.Blocked)
                    .SetProperty(dto => dto.Approved, dto.Approved)
                    .SetProperty(dto => dto.Deleted_on, TimeStamp())
                    .SetProperty(dto => dto.Deleted_by, dto.Participant_ID)
                    .SetProperty(dto => dto.Deleted, true)
                    .SetProperty(dto => dto.Updated_on, TimeStamp())
                    .SetProperty(dto => dto.Updated_by, dto.Participant_ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    participant_id = dto.Participant_ID,
                    deleted_on = TimeStamp()
                });
            }
            catch
            {
                return "Server Error: Delete Chat Permissions Failed.";
            }
        }

    }
}
