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
        private readonly Constants _Constants;
        private readonly ILogger<SelectedController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        public SelectedController(ILogger<SelectedController> logger, IConfiguration configuration, IUsersRepository UsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
            _Constants = constants;
        }

        [HttpPut("Alignment")]
        public async Task<ActionResult<string>> Change_End_User_Selected_Application_Alignment([FromBody] Selected_App_AlignmentDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.User_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;
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

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;


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

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

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

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

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

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

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

                ulong user_id =JWT.Read_Email_Account_User_ID_By_JWToken(obj.Token).Result;

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

                ulong user_id =JWT.Read_Email_Account_User_ID_By_JWToken(obj.Token).Result;

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
        public async Task<ActionResult<string>> ChangeUserSelectedStatus([FromBody] Selected_StatusDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Online_status.ToString()) || string.IsNullOrWhiteSpace(dto.Online_status.ToString()))
                    return BadRequest();

                if (dto.Online_status < 1 && dto.Online_status > 5)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                dto.User_id = user_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Status(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Theme")]
        public async Task<ActionResult<string>> ChangeUserSelectedTheme([FromBody] Selected_ThemeDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

/*                bool authentication_result = JWT.Authenticate_Client_JWT_Credentials(new JWT.JWT_AuthenticationDTO { 
                    JWT_client_address = AES.Process_Decryption(dto.JWT_client_address),
                    JWT_client_key = AES.Process_Decryption(dto.JWT_client_key),
                    JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key),
                    Language = AES.Process_Decryption(dto.Language),
                    Region = AES.Process_Decryption(dto.Region),
                    Location = AES.Process_Decryption(dto.Location),
                    Client_time = AES.Process_Decryption(dto.Client_time),
                    Controller = "Selected",
                    Action = "Theme",
                    JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result,
                    Client_id = ulong.Parse(AES.Process_Decryption(dto.ID)),
                    Login_type = AES.Process_Decryption(dto.Login_type)
                }).Result;*/
               

                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.User_id = ulong.Parse(AES.Process_Decryption(dto.ID));

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Theme(dto).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}