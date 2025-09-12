using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Twitch")]
    public class TwitchController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<TwitchController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IAES AES;
        private readonly INetwork Network;
        private readonly IValid Valid;
        private readonly IJWT JWT;
        private readonly ITwitch Twitch;

        public TwitchController(
            ILogger<TwitchController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository,
            IValid valid,
            IAES aes,
            IJWT jwt,
            INetwork network,
            ITwitch twitch,
            Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Twitch = twitch;
            Network = network;
            Valid = valid;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Validating_Twitch_Login([FromBody] Validate_Twitch_LoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption($@"{dto.Client_time}"));
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

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

                dto.Nav_lock = AES.Process_Decryption(dto.Nav_lock);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.Grid_type = AES.Process_Decryption(dto.Grid_type);

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                    Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = "Twitch",
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
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
                    Controller = "Twitch",
                    Action = "Login"
                }).Result)
                    return Conflict();

                string twitch_access_token = Twitch.Authorization_Code_Flow_Access_Token(dto.Code).Result;
                Twitch_User_Response? user_data = Twitch.Get_User_Data(twitch_access_token).Result;

                string created_email_account_token = "";
                ulong mpc_member_mpc_id = 0;
                User_Data_DTO mpc_member_data = new User_Data_DTO { };

                if (user_data.Data.IsNullOrEmpty())
                {
                    return BadRequest();
                }

                string user_email = user_data.Data[0].Email;
                string user_display_name = user_data.Data[0].DisplayName;
                ulong user_twitch_id = ulong.Parse(user_data.Data[0].Id);

                bool user_twitch_id_exists = Users_Repository.ID_Exists_In_Twitch_IDsTbl(user_twitch_id).Result;
                bool twitch_email_exists = Users_Repository.Email_Exists_In_Twitch_Email_AddressTbl(user_email).Result;

                if (user_twitch_id_exists && twitch_email_exists) {

                    mpc_member_mpc_id = Users_Repository.Read_User_ID_By_Twitch_Account_Email(user_email).Result;
                    mpc_member_data = Users_Repository.Read_User_Data_By_ID(mpc_member_mpc_id).Result;

                    await Users_Repository.Update_End_User_Login_Time_Stamp(new Login_Time_StampDTO
                    {
                        End_User_ID = mpc_member_mpc_id,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.User_agent,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });

                    await Users_Repository.Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_HistoryDTO
                    {
                        End_User_ID = mpc_member_mpc_id,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.User_agent,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });

                }  else {

                    mpc_member_data = Users_Repository.Create_Account_By_Twitch(new Complete_Twitch_RegisterationDTO
                    {
                        Twitch_Name = user_display_name,
                        Twitch_ID = user_twitch_id,
                        Email_Address = user_email,
                        Language = dto.Language,
                        Region = dto.Region,
                        Code = dto.Code,
                        Client_time = dto.Client_Time_Parsed,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Theme = byte.Parse(dto.Theme),
                        Alignment = byte.Parse(dto.Alignment),
                        Text_alignment = byte.Parse(dto.Text_alignment),
                        Nav_lock = bool.Parse(dto.Nav_lock),
                        Grid_type = byte.Parse(dto.Grid_type),
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
                        Device_ram_gb = dto.Device_ram_gb
                    }).Result;

                }

                created_email_account_token = JWT.Create_Email_Account_Token(new JWT_DTO
                {
                    End_User_ID = mpc_member_data.id,
                    User_groups = mpc_member_data.groups,
                    User_roles = mpc_member_data.roles,
                    Account_type = mpc_member_data.account_type,
                    Email_address = user_email
                }).Result;

                mpc_member_data.login_type = "TWITCH";
                mpc_member_data.email_address = user_email;

                CookieOptions cookie_options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddMinutes(_Constants.JWT_EXPIRE_TIME)
                };

                HttpContext.Session.SetString($@"AUTH|MPC:{mpc_member_data.id}|Login_Type:TWITCH", JsonSerializer.Serialize(created_email_account_token));
                Response.Cookies.Append(@$"{Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME")}", created_email_account_token, cookie_options);

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    twitch_data = user_data.Data[0],
                    mpc_data = mpc_member_data,
                    app_token = twitch_access_token
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Integrate")]
        public async Task<ActionResult<string>> Integrate_Twitch_Email([FromBody] Integrate_TwitchDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption($@"{dto.Client_time}"));
                dto.Location = AES.Process_Decryption(dto.Location);

                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

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

                dto.End_User_ID = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                    Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
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
                    Controller = "Twitch",
                    Action = "Integrate"
                }).Result)
                    return Conflict();

                string twitch_access_token = Twitch.Authorization_Code_Flow_Access_Token(dto.Code).Result;
                Twitch_User_Response? user_data = Twitch.Get_User_Data(twitch_access_token).Result;

                if (user_data.Data.IsNullOrEmpty())
                {
                    return BadRequest();
                }

                string user_email = user_data.Data[0].Email;
                string user_display_name = user_data.Data[0].DisplayName;
                ulong user_twitch_id = ulong.Parse(user_data.Data[0].Id);

                bool user_twitch_id_exists = Users_Repository.ID_Exists_In_Twitch_IDsTbl(user_twitch_id).Result;
                bool twitch_email_exists = Users_Repository.Email_Exists_In_Twitch_Email_AddressTbl(user_email).Result;

                if (user_twitch_id_exists || twitch_email_exists ) {
                    return BadRequest();
                }

                await Users_Repository.Integrate_Account_By_Twitch(new Complete_Twitch_IntegrationDTO
                {
                    End_User_ID = dto.End_User_ID,
                    Twitch_ID = user_twitch_id,
                    Email_Address = user_email,
                    Code = dto.Code
                });

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    twitch_data = user_data.Data[0],
                    app_token = twitch_access_token,
                    end_user_id = dto.End_User_ID
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Access_Token")]
        public async Task<ActionResult<string>> Get_Client_Credentials_Flow_Access_Token([FromBody] Request_Twitch_Access_TokenDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption($@"{dto.Client_time}"));
                dto.Location = AES.Process_Decryption(dto.Location);

                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

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

                dto.End_User_ID = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                    Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
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
                    Controller = "Twitch",
                    Action = "Access_Token"
                }).Result)
                    return Conflict();

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    app_token = Twitch.Get_Client_Credentials_Flow_Access_Token().Result
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}