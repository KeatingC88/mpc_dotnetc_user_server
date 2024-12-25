using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Text.Json;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Controllers.Users;
using mpc_dotnetc_user_server.Controllers.Users.AES;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace mpc_dotnetc_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        AES_RW AES_RW = new AES_RW();

        private readonly string secretKey;

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration, IUsersRepository iUsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = iUsersRepository;
        }

        [HttpPost("Confirmation")]
        public ActionResult<string> Validating_Email_Address_User_Information_Before_Submission([FromBody] Confirmation_Email_Registration_EncryptedDTO obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    BadRequest();

                obj.Email_Address = AES_RW.Process_Decryption(obj.Email_Address);
                obj.Language = AES_RW.Process_Decryption(obj.Language);
                obj.Region = AES_RW.Process_Decryption(obj.Region);

                if (!Valid.Email(obj.Email_Address) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region) ||
                    _UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result ||
                    !_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(obj.Email_Address).Result)
                    return BadRequest();

                return StatusCode(200, JsonSerializer.Serialize(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Registering_An_Email_Account_For_New_User([FromBody] Pending_Email_Registration_EncryptedDTO obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();
                
                obj.Email_Address = AES_RW.Process_Decryption(obj.Email_Address);
                obj.Language = AES_RW.Process_Decryption(obj.Language);
                obj.Region = AES_RW.Process_Decryption(obj.Region);

                if (!Valid.Email(obj.Email_Address) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region) ||
                    _UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result)
                    return Conflict();


                Pending_Email_RegistrationDTO end_user_data = new Pending_Email_RegistrationDTO
                {
                    Email_Address = obj.Email_Address,
                    Language = obj.Language,
                    Region = obj.Region,
                    Code = obj.Code
                };

                if (_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(obj.Email_Address).Result)
                    return await Task.FromResult(_UsersRepository.Update_Pending_Email_Registration_Record(end_user_data)).Result;

                return await _UsersRepository.Create_Pending_Email_Registration_Record(end_user_data);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Submit")]
        public async Task<ActionResult<string>> Submit_Login_Email_PasswordDTO([FromBody] Confirmation_Email_Registration_EncryptedDTO obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                obj.Email_Address = AES_RW.Process_Decryption(obj.Email_Address);
                obj.Language = AES_RW.Process_Decryption(obj.Language);
                obj.Password = AES_RW.Process_Decryption(obj.Password);
                obj.Region = AES_RW.Process_Decryption(obj.Region);

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result ||
                    !_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(obj.Email_Address).Result ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.Password(obj.Password) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region))
                    return BadRequest();

                Complete_Email_RegistrationDTO end_user_data = new Complete_Email_RegistrationDTO
                {
                    Email_Address = obj.Email_Address,
                    Language = obj.Language,
                    Region = obj.Region,
                    Code = obj.Code
                };

                return await Task.FromResult(_UsersRepository.Create_Account_By_Email(end_user_data)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}