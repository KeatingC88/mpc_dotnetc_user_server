using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
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
        private AES AES = new AES();
        public SelectedController(ILogger<SelectedController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPut("Alignment")]
        public async Task<ActionResult<string>> Change_End_User_Selected_Application_Alignment([FromBody] Selected_App_AlignmentDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.User_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;
                dto.Alignment = AES.Process_Decryption(dto.Alignment);

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(dto.User_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO { 
                    Alignment = dto.Alignment,
                    User_id = dto.User_id
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Text_Alignment")]
        public async Task<ActionResult<string>> ChangeEndUserSelectedTextAlignment([FromBody] Selected_App_Text_AlignmentDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;


                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();

                dto.User_id = user_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_TextAlignment(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Avatar")]
        public async Task<ActionResult<string>> ChangeUserSelectedAvatar([FromBody] Selected_AvatarDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();

                dto.User_id = user_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Avatar(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Name")]
        public async Task<ActionResult<string>> ChangeUserSelectedDisplayName([FromBody] Selected_NameDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (string.IsNullOrEmpty(dto.Name) || string.IsNullOrWhiteSpace(dto.Name))
                {
                    dto.Name = $"Recruit#{user_id}";
                }

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                dto.User_id = user_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Name(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Language")]
        public async Task<ActionResult<string>> ChangeUserSelectedLanguage([FromBody] Selected_LanguageDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Language(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("NavLock")]
        public async Task<ActionResult<string>> ChangeUserSelectedNavLock([FromBody] Selected_Navbar_LockDTO obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id =JWT.JWT.Read_User_ID_By_JWToken(obj.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();

                obj.User_id = user_id;
                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Nav_Lock(obj).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Password")]
        public async Task<ActionResult<string>> ChangeUserPassword([FromBody] Login_PasswordDTO obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id =JWT.JWT.Read_User_ID_By_JWToken(obj.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                byte[]? usersdb_SavedPasswordHash = _UsersRepository.Read_User_Password_Hash_By_ID(user_id).Result;
                byte[]? given_PasswordHash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes($"{obj.Password}"), Encoding.UTF8.GetBytes($"{obj.Email_Address}MPCSalt")).Result;

                if (usersdb_SavedPasswordHash != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(usersdb_SavedPasswordHash, given_PasswordHash))
                        return Unauthorized();

                return await Task.FromResult(_UsersRepository.Update_End_User_Password(new Login_PasswordDTO { User_id = user_id, Password = obj.Password, New_password = obj.New_password, Email_Address = obj.Email_Address })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Status")]
        public async Task<ActionResult<string>> ChangeUserSelectedStatus([FromBody] Selected_StatusDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Online_status.ToString()) || string.IsNullOrWhiteSpace(obj.Online_status.ToString()))
                    return BadRequest();

                if (obj.Online_status < 1 && obj.Online_status > 5)
                    return BadRequest();

                ulong user_id =JWT.JWT.Read_User_ID_By_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.User_id = user_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Status(obj)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Theme")]
        public async Task<ActionResult<string>> ChangeUserSelectedTheme([FromBody] Selected_ThemeDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Theme.ToString()) || string.IsNullOrWhiteSpace(obj.Theme.ToString()) ||
                    string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token))
                    return BadRequest();

                ulong user_id =JWT.JWT.Read_User_ID_By_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.User_id = user_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Theme(obj).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}