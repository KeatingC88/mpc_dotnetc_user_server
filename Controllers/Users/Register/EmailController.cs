using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Text.Json;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;

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

        [HttpPost("Exists")]
        public async Task<ActionResult<string>> Validating_Email_Exists_In_Login_Email_Address_Tbl([FromBody] Validate_Email_AddressDTO obj) {
            try {

                string email_address = AES_RW.Process_Decryption(obj.Email_Address);
                string language = AES_RW.Process_Decryption(obj.Language);
                string region = AES_RW.Process_Decryption(obj.Region);

                if (!Valid.Email(email_address) ||
                    !Valid.Language_Code(language) ||
                    !Valid.Region_Code(region))
                    return BadRequest();

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(email_address).Result)
                {
                    ulong user_id = _UsersRepository.Read_User_ID_By_Email_Address(email_address).Result;

                    await _UsersRepository.Create_Reported_Email_Post_Registration_Record(new Reported_Email_Post_RegistrationDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_ID = user_id,
                        Email_Address = email_address,
                        Language = language,
                        Region = region,
                    });

                    return Conflict();
                }

                return Ok();
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