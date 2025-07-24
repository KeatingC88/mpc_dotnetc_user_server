using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Profile;
using mpc_dotnetc_user_server.Models.Users.Community;
using mpc_dotnetc_user_server.Controllers.Interfaces;
using mpc_dotnetc_user_server.Models.Interfaces;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Load")]
    public class LoadController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<LoadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;

        public LoadController(
            ILogger<LoadController> logger, 
            IConfiguration configuration, 
            IUsers_Repository users_repository, 
            IAES aes,
            IJWT jwt,
            INetwork network,
            Constants constants
        ){
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Network = network;
        }

        [HttpPost("Token")]
        public async Task<ActionResult<string>> Renew_Token([FromBody] Renew_JWTDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
                dto.Login_type = AES.Process_Decryption(dto.Login_type);

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = await JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token);

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
    
                dto.Screen_width = AES.Process_Decryption(dto.Screen_width);
                dto.Screen_height = AES.Process_Decryption(dto.Screen_height);
                dto.RTT = AES.Process_Decryption(dto.RTT);
                dto.Orientation = AES.Process_Decryption(dto.Orientation);
                dto.Data_saver = AES.Process_Decryption(dto.Data_saver);
                dto.Color_depth = AES.Process_Decryption(dto.Color_depth);
                dto.Pixel_depth = AES.Process_Decryption(dto.Pixel_depth);
                dto.Connection_type = AES.Process_Decryption(dto.Connection_type);
                dto.Down_link = AES.Process_Decryption(dto.Down_link);
                dto.Device_ram_gb = AES.Process_Decryption(dto.Device_ram_gb);

                dto.ID = AES.Process_Decryption(dto.ID);

                var validationResult = await Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = await Network.Get_Client_Remote_Internet_Protocol_Address(),
                    Remote_Port = await Network.Get_Client_Remote_Internet_Protocol_Port(),
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Token = dto.Token,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
                    End_User_ID = dto.Client_id,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
    
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb,
                    Controller = "Load",
                    Action = "Token"
                });

                if (!validationResult)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return dto.Login_type.ToUpper() switch
                {
                    "EMAIL" => await Users_Repository.Read_Email_User_Data_By_ID(dto.End_User_ID),
                    _ => "Token Error"
                };
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}