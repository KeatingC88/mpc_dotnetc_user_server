using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Text.Json;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Controllers.Users;
using mpc_dotnetc_user_server.Controllers.Users.AES;

namespace mpc_dotnetc_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        AES_Decryptor AES = new AES_Decryptor();

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration, IUsersRepository iUsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = iUsersRepository;
        }

        [HttpPost("Confirmation")]
        public ActionResult<string> Validating_Email_Address_User_Information_Before_Submission([FromBody] Confirmation_Email_RegistrationDTO obj)
        {
            try
            {
                if (!ModelState.IsValid ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region) ||
                    !_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(obj.Email_Address).Result ||
                    _UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result ||
                    !_UsersRepository.Confirmation_Code_Exists_In_Pending_Email_Address_RegistrationTbl (obj.Code).Result)
                    return BadRequest();

                return StatusCode(200, JsonSerializer.Serialize(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Register")]
        public string Registering_An_Email_Account_For_New_User([FromBody] EncryptedPayload obj)
        {            
            try
            {                
                var foo = AES.Process(obj.Data);
                return foo;

/*                if (!ModelState.IsValid ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region))
                    return BadRequest();

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result)
                    return StatusCode(409);

                if (_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(obj.Email_Address).Result)
                    return await Task.FromResult(_UsersRepository.Update_Pending_Email_Registration_Record(obj)).Result;

                return await _UsersRepository.Create_Pending_Email_Registration_Record(obj);*/

            } catch (Exception e) {
                return e.Message.ToString();
                //return StatusCode(500, $"{e.Message}");
            }
        }
        public class EncryptedPayload
        {
            public string Data { get; set; }
        }

        [HttpPost("Submit")]
        public async Task<ActionResult<string>> Submit_Login_Email_PasswordDTO([FromBody] Complete_Email_RegistrationDTO obj)
        {
            try
            {
                if (!ModelState.IsValid ||
                    _UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.Password(obj.Password) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region))
                    return BadRequest();

                return await Task.FromResult(_UsersRepository.Create_Account_By_Email(obj)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}