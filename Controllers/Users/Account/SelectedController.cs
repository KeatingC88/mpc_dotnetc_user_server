using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Selections;
using System.Text;


namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Selected")]
    public class SelectedController : ControllerBase
    {
        private readonly ILogger<SelectedController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        public SelectedController(ILogger<SelectedController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Update_User_Selected_Alignment(obj)).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Update_User_Selected_TextAlignment(obj)).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Update_User_Avatar(obj)).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (string.IsNullOrEmpty(obj.Display_name) || string.IsNullOrWhiteSpace(obj.Display_name))
                {
                    obj.Display_name = $"Recruit#{user_id}";
                }

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;
                obj.Display_name = obj.Display_name;

                return await Task.FromResult(_UsersRepository.Update_User_Display_Name(obj)).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                return await Task.FromResult(_UsersRepository.Update_User_Selected_Language(
                    new DTO
                    {
                        User_ID = user_id,
                        Language_code = dto.Language.Substring(0, 2),
                        Region_code = dto.Language.Substring(3, 2),
                        Updated_by = user_id
                    }
                )).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Update_User_Selected_Nav_Lock(obj).Result);
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
                    string.IsNullOrEmpty(obj.Email_Address) || string.IsNullOrWhiteSpace(obj.Email_Address))
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                byte[]? usersdb_SavedPasswordHash = _UsersRepository.Get_User_Password_Hash_By_ID(user_id).Result;
                byte[]? given_PasswordHash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{obj.Email_Address}MPCSalt")).Result;

                if (usersdb_SavedPasswordHash != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(usersdb_SavedPasswordHash, given_PasswordHash))
                        return Unauthorized();

                return await Task.FromResult(_UsersRepository.Update_User_Password(new DTO { ID = user_id, Password = obj.Password, New_password = obj.New_password, Email_Address = obj.Email_Address })).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Update_User_Selected_Status(obj)).Result;
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

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Update_User_Selected_Theme(obj).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}