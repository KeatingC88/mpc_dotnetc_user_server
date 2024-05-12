using Microsoft.AspNetCore.Mvc;
//JWT Stuff...
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Text.RegularExpressions;
using dotnet_user_server.Models.Users;
using dotnet_user_server.Models.Users.Selections;

namespace dotnet_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Selected")]
    public class SelectedController : ControllerBase
    {
        private readonly ILogger<SelectedController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public SelectedController(ILogger<SelectedController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }

        [HttpPut("Alignment")]
        public async Task<ActionResult<string>> ChangeEndUserSelectedAlignment([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(obj.Token.ToString()) || string.IsNullOrWhiteSpace(obj.Token.ToString()) ||
                    string.IsNullOrWhiteSpace(obj.Alignment.ToString()) || string.IsNullOrWhiteSpace(obj.Alignment.ToString())
                    )
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Update_User_Selected_Alignment(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Text_Alignment")]
        public async Task<ActionResult<string>> ChangeEndUserSelectedTextAlignment([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(obj.Token.ToString()) || string.IsNullOrWhiteSpace(obj.Token.ToString()) ||
                    string.IsNullOrWhiteSpace(obj.Alignment.ToString()) || string.IsNullOrWhiteSpace(obj.Alignment.ToString())
                    )
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Update_User_Selected_TextAlignment(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Avatar")]
        public async Task<ActionResult<string>> ChangeUserSelectedAvatar([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Avatar_url_path) || string.IsNullOrWhiteSpace(obj.Avatar_url_path))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Update_User_Avatar(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("DisplayName")]
        public async Task<ActionResult<string>> ChangeUserSelectedDisplayName([FromBody] DTO obj)
        {
            try
            {

                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (string.IsNullOrEmpty(obj.Display_name) || string.IsNullOrWhiteSpace(obj.Display_name))
                {
                    obj.Display_name = $"Recruit#{user_id}";
                }

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;
                obj.Display_name = obj.Display_name;

                return await Task.FromResult(UsersDbC.Update_User_Display_Name(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Language")]
        public async Task<ActionResult<string>> ChangeUserSelectedLanguage([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) || string.IsNullOrEmpty(dto.Language) || string.IsNullOrWhiteSpace(dto.Language))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                return await Task.FromResult(UsersDbC.Update_User_Selected_Language(
                    new Selected_LanguageTbl
                    {
                        User_ID = user_id,
                        Language_code = dto.Language.Substring(0, 2),
                        Region_code = dto.Language.Substring(3, 2),
                        Updated_by = user_id
                    }
                ));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("NavLock")]
        public async Task<ActionResult<string>> ChangeUserSelectedNavLock([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Nav_lock.ToString()) || string.IsNullOrWhiteSpace(obj.Nav_lock.ToString())
                    )
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Update_User_Selected_Nav_Lock(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Password")]
        public async Task<ActionResult<bool>> ChangeUserPassword([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.ID.ToString()) || string.IsNullOrWhiteSpace(obj.ID.ToString()) ||
                    string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Language) || string.IsNullOrWhiteSpace(obj.Language) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password) ||
                    string.IsNullOrWhiteSpace(obj.New_password) || string.IsNullOrEmpty(obj.New_password) ||
                    string.IsNullOrEmpty(obj.Email_address) || string.IsNullOrWhiteSpace(obj.Email_address))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                byte[]? usersdb_SavedPasswordHash = UsersDbC.Get_User_Password_Hash_By_ID(user_id);
                byte[]? given_PasswordHash = UsersDbC.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{obj.Email_address}MPCSalt"));

                if (usersdb_SavedPasswordHash != null)
                    if (!UsersDbC.Compare_Password_Byte_Arrays(usersdb_SavedPasswordHash, given_PasswordHash))
                        return Unauthorized();

                return await Task.FromResult(UsersDbC.Update_User_Password(new DTO { ID = user_id, Password = obj.Password, New_password = obj.New_password, Email_address = obj.Email_address }));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Status")]
        public async Task<ActionResult<string>> ChangeUserSelectedStatus([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Online_status.ToString()) || string.IsNullOrWhiteSpace(obj.Online_status.ToString()))
                    return BadRequest();

                if (obj.Online_status < 1 && obj.Online_status > 5)
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Update_User_Selected_Status(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Theme")]
        public async Task<ActionResult<string>> ChangeUserSelectedTheme([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Theme.ToString()) || string.IsNullOrWhiteSpace(obj.Theme.ToString()) ||
                    string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Update_User_Selected_Theme(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}