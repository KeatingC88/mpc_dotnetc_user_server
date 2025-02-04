using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Authentication.Report;


namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Load")]
    public class LoadController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<LoadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        

        public LoadController(ILogger<LoadController> logger, IConfiguration configuration, IUsersRepository UsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
            _Constants = constants;
        }

        [HttpPost("All_Users")]
        public async Task<ActionResult<string>> LoadAllUsers([FromBody] Load_All_UsersDTO dto)
        {
            try {

                if (!ModelState.IsValid)
                    return BadRequest();

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.ID = AES.Process_Decryption(dto.ID);
                ulong user_id_from_jwt = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Client_id = ulong.Parse(dto.ID),
                        JWT_id = user_id_from_jwt,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-Server Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = user_id_from_jwt,
                        Login_type = dto.Login_type,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address
                    });
                }

                return await _UsersRepository.Read_Users();
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

/*        [HttpPost("User")]
        public async Task<ActionResult<string>> LoadUser([FromBody] UserDTO dto)
        {
            try
            {
                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                UserDTO obj = new UserDTO
                {
                    ID = user_id,
                    Token = dto.Token
                };

                return await _UsersRepository.Read_Email_User_Data_By_ID(obj.ID);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }*/

/*        [HttpPost("User/Profile")]
        public async Task<ActionResult<string>> LoadUserProfile([FromBody] Read_User_ProfileDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong client_given_user_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                ulong jwt_given_user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    dto.Language = AES.Process_Decryption(dto.Language);
                    dto.Region = AES.Process_Decryption(dto.Region);
                    dto.Location = AES.Process_Decryption(dto.Location);
                    dto.Client_time = AES.Process_Decryption(dto.Client_time);
                    await _UsersRepository.Insert_Report_Failed_Load_Users_HistoryTbl(new Report_Failed_Load_Users_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_ID = client_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "JWT Mismatch for Load_User_Profile"
                    });
                    return Conflict();
                }

                if (client_given_user_id != jwt_given_user_id)
                {
                    dto.Language = AES.Process_Decryption(dto.Language);
                    dto.Region = AES.Process_Decryption(dto.Region);
                    dto.Location = AES.Process_Decryption(dto.Location);
                    dto.Client_time = AES.Process_Decryption(dto.Client_time);
                    await _UsersRepository.Insert_Report_Failed_User_ID_HistoryTbl(new Report_Failed_User_ID_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = client_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "User Client ID and JWT Mismatch Load_User_Profile."
                    });
                    return Conflict();
                }

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(client_given_user_id).Result)
                {
                    dto.Language = AES.Process_Decryption(dto.Language);
                    dto.Region = AES.Process_Decryption(dto.Region);
                    dto.Location = AES.Process_Decryption(dto.Location);
                    dto.Client_time = AES.Process_Decryption(dto.Client_time);
                    await _UsersRepository.Insert_Report_Failed_User_ID_HistoryTbl(new Report_Failed_User_ID_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = client_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "User Client ID is Not Found for Load_User_Profile."
                    });
                    return NotFound();
                }

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(jwt_given_user_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_User_ID_HistoryTbl(new Report_Failed_User_ID_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = jwt_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "User JWT ID NotFound for Load_User_Profile."
                    });
                    return NotFound();
                }

                return await _UsersRepository.Read_User_Profile_By_ID(client_given_user_id);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }*/
    }//Controller.
}