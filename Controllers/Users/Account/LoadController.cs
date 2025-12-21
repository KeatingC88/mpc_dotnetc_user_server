using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Security.JWT;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Load")]
    public class LoadController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<LoadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository_Read Users_Repository_Read;
        private readonly ISystem_Tampering System_Tampering;
        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;

        public LoadController(
            ILogger<LoadController> logger, 
            IConfiguration configuration, 
            IUsers_Repository_Read users_repository_read, 
            IAES aes,
            IJWT jwt,
            INetwork network,
            Constants constants
        ){
            _logger = logger;
            _configuration = configuration;
            Users_Repository_Read = users_repository_read;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Network = network;
        }

        [HttpPost("Session")]
        public async Task<ActionResult<string>> Renew_Session_Token([FromBody] Renew_Session_JWT_DTO dto)
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
                dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
                dto.Login_type = AES.Process_Decryption(dto.Login_type);

                dto.End_User_ID = long.Parse(AES.Process_Decryption(dto.ID));

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = dto.Client_user_agent;

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

                var validationResult = await System_Tampering.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
                {
                    Remote_IP = await Network.Get_Client_Remote_Internet_Protocol_Address(),
                    Remote_Port = await Network.Get_Client_Remote_Internet_Protocol_Port(),
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_time = dto.Client_Time_Parsed,
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
                    Action = "Session"
                });

                if (!validationResult)
                    return Conflict();

                
                User_Token_Data_DTO user_data = await Users_Repository_Read.Read_Require_Token_Data_By_ID(dto.End_User_ID);
                CookieOptions cookie_options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddMinutes(_Constants.JWT_EXPIRE_TIME)
                };

                string created_email_account_token = JWT.Create_Email_Account_Token(new JWT_DTO
                {
                    End_User_ID = user_data.id,
                    User_groups = user_data.groups,
                    User_roles = user_data.roles,
                    Account_type = user_data.account_type,
                    Email_address = user_data.email_address
                }).Result;

                HttpContext.Session.Remove($@"AUTH|MPC:{user_data.id}|Login_Type={dto.Login_type}");
                HttpContext.Session.SetString($@"AUTH|MPC:{user_data.id}|Login_Type={dto.Login_type}", JsonSerializer.Serialize(created_email_account_token));
                Response.Cookies.Append(@$"{Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME")}", created_email_account_token, cookie_options);

                return await Task.FromResult(Ok());
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}