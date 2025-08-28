using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Services;
using System.Net.Http.Headers;
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


        public TwitchController(
            ILogger<TwitchController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository,
            IValid valid,
            IAES aes,
            IJWT jwt,
            INetwork network,
            Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Network = network;
            Valid = valid;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<string>> Validating_Twitch_Login([FromBody] Validate_TwitchDTO dto)
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
                    Action = "Exists"
                }).Result)
                    return Conflict();

                using var client = new HttpClient();
                var response = await client.PostAsync("https://id.twitch.tv/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID") ?? string.Empty },
                    { "client_secret", Environment.GetEnvironmentVariable("TWITCH_CLIENT_SECRET") ?? string.Empty },
                    { "code", dto.Code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", Environment.GetEnvironmentVariable("TWITCH_CLIENT_REDIRECT_URI") ?? string.Empty }
                }));

                var content = await response.Content.ReadAsStringAsync();

                var tokenResponse = JsonSerializer.Deserialize<Twitch_Token_Response>(content);
                if (tokenResponse == null)
                {
                    return StatusCode(500, "Invalid Twitch Response 1.");
                }

                var twitch_client = new HttpClient();
                twitch_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResponse.AccessToken ?? "");
                twitch_client.DefaultRequestHeaders.Add("Client-Id", Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID") ?? string.Empty);
                var twitch_response = await twitch_client.GetAsync("https://api.twitch.tv/helix/users");
                
                if (!twitch_response.IsSuccessStatusCode)
                {
                    return StatusCode(500, "Invalid Twitch Response 2.");
                }

                var userJson = await twitch_response.Content.ReadAsStringAsync();
                
                var userData = JsonSerializer.Deserialize<Twitch_User_Response>(userJson, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                string user_email = "";
                string created_email_account_token = "";
                ulong mpc_member_mpc_id = 0;

                if (userData == null || userData.Data == null || userData.Data[0].Id == null || userData.Data[0].Email == null)
                {
                    return StatusCode(500, "Invalid Twitch Response 3.");

                }

                user_email = userData.Data[0].Email;

                bool user_id_exists = Users_Repository.ID_Exists_In_Twitch_IDsTbl(ulong.Parse(userData.Data[0].Id)).Result;
                bool twitch_email_exists = Users_Repository.Email_Exists_In_Twitch_Email_AddressTbl(user_email).Result;

                if (user_id_exists && twitch_email_exists)
                {

                    mpc_member_mpc_id = Users_Repository.Read_User_ID_By_Twitch_Account_Email(userData.Data[0].Email).Result;
                    User_Data_DTO mpc_member_data = Users_Repository.Read_User_Data_By_ID(mpc_member_mpc_id).Result;

                    created_email_account_token = JWT.Create_Email_Account_Token(new JWT_DTO
                    {
                        End_User_ID = mpc_member_data.id,
                        User_groups = mpc_member_data.groups,
                        User_roles = mpc_member_data.roles,
                        Account_type = mpc_member_data.account_type,
                        Email_address = user_email
                    }).Result;

                    mpc_member_data.login_type = "TWITCH";

                    CookieOptions cookie_options = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Path = "/",
                        Expires = DateTime.UtcNow.AddMinutes(_Constants.JWT_EXPIRE_TIME)
                    };

                    HttpContext.Session.SetString($@"AUTH|MPC:{mpc_member_data.id}|TWITCH:{mpc_member_data.twitch_id}|EMAIL_ADDRESS:{mpc_member_data.twitch_email_address}", JsonSerializer.Serialize(userData.Data[0]));
                    Response.Cookies.Append(@$"{Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME")}", created_email_account_token, cookie_options);

                    return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                    {
                        twitch_data = userData.Data[0],
                        mpc_data = mpc_member_data,
                        app_token = tokenResponse.AccessToken
                    }))));

                }
                else
                {

                    User_Data_DTO mpc_member_data = Users_Repository.Create_Account_By_Twitch(new Complete_Twitch_RegisterationDTO
                    {
                        Twitch_Name = userData.Data[0].DisplayName,
                        Twitch_ID = ulong.Parse(userData.Data[0].Id),
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

                    created_email_account_token = JWT.Create_Email_Account_Token(new JWT_DTO
                    {
                        End_User_ID = mpc_member_data.id,
                        User_groups = mpc_member_data.groups,
                        User_roles = mpc_member_data.roles,
                        Account_type = mpc_member_data.account_type,
                        Email_address = user_email
                    }).Result;

                    mpc_member_data.login_type = "TWITCH";

                    CookieOptions cookie_options = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,
                        SameSite = SameSiteMode.Lax,
                        Path = "/",
                        Expires = DateTime.UtcNow.AddMinutes(_Constants.JWT_EXPIRE_TIME)
                    };

                    HttpContext.Session.SetString($@"AUTH|MPC:{mpc_member_data.id}|TWITCH:{mpc_member_data.twitch_id}|EMAIL_ADDRESS:{mpc_member_data.twitch_email_address}", JsonSerializer.Serialize(mpc_member_data));
                    Response.Cookies.Append(@$"{Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME")}", created_email_account_token, cookie_options);

                    return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                    {
                        twitch_data = userData.Data[0],
                        mpc_data = mpc_member_data,
                        app_token = tokenResponse.AccessToken
                    }))));
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}