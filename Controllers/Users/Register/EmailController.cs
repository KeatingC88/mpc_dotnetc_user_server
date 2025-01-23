using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Pending.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Report;

namespace mpc_dotnetc_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        AES AES = new AES();

        private readonly string secretKey;

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration, IUsersRepository iUsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = iUsersRepository;
            _Constants = constants;
        }

        [HttpPost("Confirmation")]
        public async Task<ActionResult<string>> Validating_Email_Address_User_Information_Before_Submission([FromBody] Confirmation_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    await _UsersRepository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "JWT Mismatch"
                    });
                    return Conflict();
                }


                if (!Valid.Email(dto.Email_Address) ||
                    !Valid.Language_Code(dto.Language) ||
                    !Valid.Region_Code(dto.Region) ||
                    _UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result ||
                    !_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(dto.Email_Address).Result)
                    return BadRequest();

                return StatusCode(200, JsonSerializer.Serialize(dto));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Exists")]
        public async Task<ActionResult<string>> Validating_Email_Exists_In_Login_Email_Address_Tbl([FromBody] Validate_Email_AddressDTO dto) 
        {
            try {

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Location = AES.Process_Decryption(dto.Location);

                if (!Valid.Email(dto.Email_Address) ||
                    !Valid.Language_Code(dto.Language) ||
                    !Valid.Region_Code(dto.Region))
                    return BadRequest();

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                {
                    ulong user_id = _UsersRepository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;

                    await _UsersRepository.Insert_Report_Email_RegistrationTbl(new Report_Email_RegistrationDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = user_id,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "Email Already Exists."
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
                dto.Client_time = AES.Process_Decryption(dto.Client_time.ToString());
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY || 
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    await _UsersRepository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "JWT Mismatch"
                    });
                    return Conflict();
                }

                if (!Valid.Email(dto.Email_Address) || !Valid.Language_Code(dto.Language) || !Valid.Region_Code(dto.Region)) {
                    return BadRequest();
                }

                await _UsersRepository.Insert_Pending_Email_Registration_History_Record(new Pending_Email_Registration_HistoryDTO
                {
                    Email_Address = dto.Email_Address,
                    Language = dto.Language,
                    Region = dto.Region,
                    Client_time = ulong.Parse(dto.Client_time),
                    Location = dto.Location,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    Code = dto.Code
                });

                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result) 
                {
                    await _UsersRepository.Insert_Report_Email_RegistrationTbl(new Report_Email_RegistrationDTO {
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Client_time = ulong.Parse(dto.Client_time),
                        Location = dto.Location,
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                    });
                    return Conflict();
                }

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
        public async Task<ActionResult<string>> Submit_Login_Email_PasswordDTO([FromBody] Submit_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address); 
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Password = AES.Process_Decryption(dto.Password);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Nav_lock = AES.Process_Decryption(dto.Nav_lock);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.Grid_type = AES.Process_Decryption(dto.Grid_type);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY || 
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    await _UsersRepository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "JWT Mismatch"
                    });
                    return Conflict();
                }


                if (_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result ||
                    !_UsersRepository.Email_Exists_In_Pending_Email_RegistrationTbl(dto.Email_Address).Result ||
                    !Valid.Email(dto.Email_Address) ||
                    !Valid.Password(dto.Password) ||
                    !Valid.Language_Code(dto.Language) ||
                    !Valid.Region_Code(dto.Region))
                    return BadRequest();

                return await Task.FromResult(_UsersRepository.Create_Account_By_Email(new Complete_Email_RegistrationDTO
                {
                    Email_Address = dto.Email_Address,
                    Language = dto.Language,
                    Region = dto.Region,
                    Code = dto.Code,
                    Client_time = dto.Client_time,
                    Location = dto.Location,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    Theme = byte.Parse(dto.Theme),
                    Alignment = byte.Parse(dto.Alignment),
                    Text_alignment = byte.Parse(dto.Text_alignment),
                    Nav_lock = bool.Parse(dto.Nav_lock),
                    Password = dto.Password,
                    Grid_type = byte.Parse(dto.Grid_type)
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}