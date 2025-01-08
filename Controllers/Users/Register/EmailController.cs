using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email;
using mpc_dotnetc_user_server.Models.Users.Notification.Email;

namespace mpc_dotnetc_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        AES AES = new AES();

        private readonly string secretKey;

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
                if (!ModelState.IsValid)
                    BadRequest();

                obj.Email_Address = AES.Process_Decryption(obj.Email_Address);
                obj.Language = AES.Process_Decryption(obj.Language);
                obj.Region = AES.Process_Decryption(obj.Region);
                obj.Client_time = AES.Process_Decryption(obj.Client_time);
                obj.Location = AES.Process_Decryption(obj.Location);

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
        public async Task<ActionResult<string>> Validating_Email_Exists_In_Login_Email_Address_Tbl([FromBody] Validate_Email_AddressDTO obj) 
        {
            try {

                string email_address = AES.Process_Decryption(obj.Email_Address);
                string language = AES.Process_Decryption(obj.Language);
                string region = AES.Process_Decryption(obj.Region);

                if (!Valid.Email(email_address) ||
                    !Valid.Language_Code(language) ||
                    !Valid.Region_Code(region))
                    return BadRequest();

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(email_address).Result)
                {
                    ulong user_id = _UsersRepository.Read_User_ID_By_Email_Address(email_address).Result;

                    await _UsersRepository.Create_Reported_Email_Registration_Record(new Reported_Email_RegistrationDTO
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
        public async Task<ActionResult<string>> Registering_An_Email_Account_For_New_User([FromBody] Pending_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();
                
                dto.Email_Address = AES.Process_Decryption(dto.Email_Address);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Location = AES.Process_Decryption(dto.Location);

                if (!Valid.Email(dto.Email_Address) ||
                    !Valid.Language_Code(dto.Language) ||
                    !Valid.Region_Code(dto.Region) ||
                    _UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                    return Conflict();

                await _UsersRepository.Insert_Pending_Email_Registration_History_Record(new Pending_Email_Registration_HistoryDTO
                {
                    Email_Address = dto.Email_Address,
                    Language = dto.Language,
                    Region = dto.Region,
                    Client_time = dto.Client_time,
                    Location = dto.Location,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    Code = dto.Code
                });

                if (_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(dto.Email_Address).Result)
                    return await Task.FromResult(_UsersRepository.Update_Pending_Email_Registration_Record(dto)).Result;

                return await _UsersRepository.Create_Pending_Email_Registration_Record(new Pending_Email_RegistrationDTO {
                    Email_Address = dto.Email_Address,
                    Language = dto.Language,
                    Region = dto.Region,
                    Client_time = dto.Client_time,
                    Location = dto.Location,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    Code = dto.Code
                });

            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Submit")]
        public async Task<ActionResult<string>> Submit_Login_Email_PasswordDTO([FromBody] Submit_Email_RegistrationDTO obj)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                obj.Email_Address = AES.Process_Decryption(obj.Email_Address);
                obj.Language = AES.Process_Decryption(obj.Language);
                obj.Password = AES.Process_Decryption(obj.Password);
                obj.Region = AES.Process_Decryption(obj.Region);
                obj.Client_time = AES.Process_Decryption(obj.Client_time);
                obj.Location = AES.Process_Decryption(obj.Location);
                obj.Nav_Lock = AES.Process_Decryption(obj.Nav_Lock);
                obj.Alignment = AES.Process_Decryption(obj.Alignment);
                obj.Theme = AES.Process_Decryption(obj.Theme);

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result ||
                    !_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(obj.Email_Address).Result ||
                    !Valid.Email(obj.Email_Address) ||
                    !Valid.Password(obj.Password) ||
                    !Valid.Language_Code(obj.Language) ||
                    !Valid.Region_Code(obj.Region))
                    return BadRequest();

                return await Task.FromResult(_UsersRepository.Create_Account_By_Email(new Complete_Email_RegistrationDTO
                {
                    Email_Address = obj.Email_Address,
                    Language = obj.Language,
                    Region = obj.Region,
                    Code = obj.Code,
                    Client_time = obj.Client_time,
                    Location = obj.Location,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    Theme = byte.Parse(obj.Theme),
                    Alignment = byte.Parse(obj.Alignment),
                    Nav_lock = bool.Parse(obj.Nav_Lock)
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}