using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration, IUsersRepository iUsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = iUsersRepository;
        }
        [HttpPost("Confirmation")]
        public ActionResult<string> Email_Account_Confirmation([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_Address) ||
                    string.IsNullOrWhiteSpace(obj.Email_Address) ||
                    string.IsNullOrEmpty(obj.Code) ||
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    !Valid.Email(obj.Email_Address) ||
                    !_UsersRepository.Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(obj.Email_Address).Result ||
                    _UsersRepository.Email_Exists_In_Login_Email_Address_Tbl(obj.Email_Address).Result ||
                    !_UsersRepository.Confirmation_Code_Exists_In_Not_Confirmed_Email_Address_Tbl(obj.Code).Result)
                    return BadRequest();

                return StatusCode(200, JsonSerializer.Serialize(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Email_Account_Register([FromBody] DTO obj)
        {            
            try
            {
                if (string.IsNullOrEmpty(obj.Email_Address) ||
                    string.IsNullOrWhiteSpace(obj.Email_Address) ||
                    string.IsNullOrWhiteSpace(obj.Language) ||
                    string.IsNullOrEmpty(obj.Code) ||
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.LanguageRegion(obj.Language))
                    return BadRequest();

                if (_UsersRepository.Email_Exists_In_Login_Email_Address_Tbl(obj.Email_Address).Result)
                    return StatusCode(409);

                if (_UsersRepository.Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(obj.Email_Address).Result)
                    return await Task.FromResult(_UsersRepository.Update_Unconfirmed_Email(obj)).Result;

                return await _UsersRepository.Create_Unconfirmed_Email(obj);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Submit")]
        public async Task<ActionResult<string>> Submit_Email_Password([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_Address) ||
                    string.IsNullOrWhiteSpace(obj.Email_Address) ||
                    string.IsNullOrEmpty(obj.Password) ||
                    string.IsNullOrWhiteSpace(obj.Password) ||
                    string.IsNullOrEmpty(obj.Language) ||
                    string.IsNullOrWhiteSpace(obj.Language) ||
                    _UsersRepository.Email_Exists_In_Login_Email_Address_Tbl(obj.Email_Address).Result ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.Password(obj.Password) ||
                    !Valid.LanguageRegion(obj.Language))
                    return BadRequest();

                return await Task.FromResult(_UsersRepository.Create_Account_By_Email(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.