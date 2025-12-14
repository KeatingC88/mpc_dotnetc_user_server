
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Interfaces.Social;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Security.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Controllers.Users.Social.Media
{
    [ApiController]
    [Route("api/Discord")]
    public class DiscordController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<DiscordController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IUsers_Repository_Read Users_Repository_Read;
        private readonly IUsers_Repository_Update Users_Repository_Update;
        private readonly IUsers_Repository_Create Users_Repository_Create;
        private readonly IUsers_Repository_Integrate Users_Repository_Integrate;
        private readonly IAES AES;
        private readonly INetwork Network;
        private readonly IValid Valid;
        private readonly IJWT JWT;
        private readonly IDiscord Discord;

        public DiscordController(
            ILogger<DiscordController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository,
            IUsers_Repository_Read users_repository_read,
            IUsers_Repository_Update users_repository_update,
            IUsers_Repository_Create users_repository_create,
            IUsers_Repository_Integrate users_repository_integrate,
            IValid valid,
            IAES aes,
            IJWT jwt,
            INetwork network,
            IDiscord discord,
            Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            Users_Repository_Read = users_repository_read;
            Users_Repository_Update = users_repository_update;
            Users_Repository_Create = users_repository_create;
            Users_Repository_Integrate = users_repository_integrate;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Network = network;
            Discord = discord;
            Valid = valid;
        }


        [HttpPost("Login")]
        public async Task<ActionResult<string>> Validating_Discord_Login([FromBody] Validate_Discord_LoginDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption($@"{dto.Client_time}"));
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

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                    Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = "Discord",
                    Client_time = dto.Client_Time_Parsed,
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
                    Controller = "Discord",
                    Action = "Login"
                }).Result)
                    return Conflict();

                string discord_access_token = Discord.Authorization_Code_Flow_Access_Token(dto.Code).Result;
                Discord_User_Response? user_data = Discord.Get_User_Data(discord_access_token).Result;

                string created_email_account_token = "";
                long mpc_id = 0;
                User_Data_DTO mpc_member_data = new User_Data_DTO { };

                if (user_data.Data.IsNullOrEmpty())
                {
                    return BadRequest();
                }

                string user_email = user_data.Data[0].Email;
                string user_display_name = user_data.Data[0].DisplayName;
                long user_discord_id = long.Parse(user_data.Data[0].Id);
                string user_discord_login_name = user_data.Data[0].Login;

                bool user_discord_id_exists = Users_Repository_Read.Read_ID_Exists_In_Discord_IDs(user_discord_id).Result;
                bool discord_email_exists = Users_Repository_Read.Read_Email_Exists_In_Discord_Email_Address(user_email).Result;

                if (user_discord_id_exists && discord_email_exists)
                {

                    mpc_id = Users_Repository_Read.Read_User_ID_By_Discord_Account_Email(user_email).Result;
                    mpc_member_data = Users_Repository_Read.Read_User_Data_By_ID(mpc_id).Result;

                    await Users_Repository_Update.Update_End_User_Login_Time_Stamp(new Login_Time_Stamp
                    {
                        End_User_ID = mpc_id,
                        Client_time = dto.Client_Time_Parsed,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
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

                    await Users_Repository_Create.Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_History
                    {
                        End_User_ID = mpc_id,
                        Client_time = dto.Client_Time_Parsed,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
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

                }
                else
                {

                    mpc_member_data = Users_Repository_Create.Create_Account_By_Discord(new Complete_Discord_Registeration
                    {
                        Discord_Name = user_display_name,
                        Discord_User_Name = user_discord_login_name,
                        Discord_ID = user_discord_id,
                        Email_Address = user_email,
                        Language = dto.Language,
                        Region = dto.Region,
                        Code = dto.Code,
                        Client_time = dto.Client_Time_Parsed,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
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
                    discord_data = user_data.Data[0],
                    mpc_data = mpc_member_data,
                    app_token = discord_access_token
                }))));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Integrate")]
        public async Task<ActionResult<string>> Integrate_Discord_Email([FromBody] Integrate_DiscordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption($@"{dto.Client_time}"));
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

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
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
                    Client_time = dto.Client_Time_Parsed,
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
                    Controller = "Discord",
                    Action = "Integrate"
                }).Result)
                    return Conflict();

                string discord_access_token = Discord.Authorization_Code_Flow_Access_Token(dto.Code).Result;
                Discord_User_Response? user_data = Discord.Get_User_Data(discord_access_token).Result;

                if (user_data.Data.IsNullOrEmpty())
                {
                    return BadRequest();
                }

                string user_email = user_data.Data[0].Email;
                string user_display_name = user_data.Data[0].DisplayName;
                long user_discord_id = long.Parse(user_data.Data[0].Id);
                string user_discord_login_name = user_data.Data[0].Login;

                bool user_discord_id_exists = Users_Repository_Read.Read_ID_Exists_In_Discord_IDs(user_discord_id).Result;
                bool discord_email_exists = Users_Repository_Read.Read_Email_Exists_In_Discord_Email_Address(user_email).Result;

                if (user_discord_id_exists || discord_email_exists || user_discord_login_name.IsNullOrEmpty())
                {
                    return BadRequest();
                }

                await Users_Repository_Integrate.Integrate_Account_By_Discord(new Complete_Discord_Integration
                {
                    End_User_ID = dto.End_User_ID,
                    Discord_ID = user_discord_id,
                    Email_Address = user_email,
                    Discord_User_Name = user_discord_login_name,
                    Code = dto.Code
                });

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    discord_data = user_data.Data[0],
                    app_token = discord_access_token,
                    end_user_id = dto.End_User_ID
                }))));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Access_Token")]
        public async Task<ActionResult<string>> Get_Client_Credentials_Flow_Access_Token([FromBody] Request_Discord_Access_TokenDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption($@"{dto.Client_time}"));
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

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
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
                    Client_time = dto.Client_Time_Parsed,
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
                    Controller = "Discord",
                    Action = "Access_Token"
                }).Result)
                    return Conflict();

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    app_token = Discord.Get_Client_Credentials_Flow_Access_Token().Result
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}