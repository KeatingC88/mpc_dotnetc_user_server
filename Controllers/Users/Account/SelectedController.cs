using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Password_Change;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using System.Text;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Selected")]
    public class SelectedController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<SelectedController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        public SelectedController(ILogger<SelectedController> logger, IConfiguration configuration, IUsersRepository UsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
            _Constants = constants;
        }

        [HttpPut("Alignment")]
        public async Task<ActionResult<string>> Change_End_User_Selected_Application_Alignment([FromBody] Selected_App_AlignmentDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.Alignment = AES.Process_Decryption(dto.Alignment);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Alignment"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO { 
                    Alignment = dto.Alignment,
                    End_User_ID = dto.End_User_ID
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Text_Alignment")]
        public async Task<ActionResult<string>> ChangeEndUserSelectedTextAlignment([FromBody] Selected_App_Text_AlignmentDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Text_Alignment"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_TextAlignment(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Avatar")]
        public async Task<ActionResult<string>> ChangeUserSelectedAvatar([FromBody] Selected_AvatarDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
                dto.Avatar_url_path = AES.Process_Decryption(dto.Avatar_url_path);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Avatar"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Avatar(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Avatar_Title")]
        public async Task<ActionResult<string>> ChangeUserSelectedAvatar([FromBody] Selected_Avatar_TitleDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.Avatar_title = AES.Process_Decryption(dto.Avatar_title);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Avatar"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Avatar_Title(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Name")]
        public async Task<ActionResult<string>> ChangeUserSelectedDisplayName([FromBody] Selected_NameDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
                dto.Name = AES.Process_Decryption(dto.Name);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Name"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Name(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Grid")]
        public async Task<ActionResult<string>> ChangeUserSelectedGridType([FromBody] Selected_App_Grid_TypeDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
                dto.Grid = AES.Process_Decryption(dto.Grid);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Grid Type"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Grid_Type(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Language")]
        public async Task<ActionResult<string>> ChangeUserSelectedLanguage([FromBody] Selected_LanguageDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Language"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Language(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("NavLock")]
        public async Task<ActionResult<string>> ChangeUserSelectedNavLock([FromBody] Selected_Navbar_LockDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.Locked = AES.Process_Decryption(dto.Locked);
                dto.ID = AES.Process_Decryption(dto.ID);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "NavLock"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Nav_Lock(dto).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Password")]
        public async Task<ActionResult<string>> ChangeUserPassword([FromBody] Password_ChangeDTO dto)
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
                    dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                    dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                    dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                    dto.Window_height = AES.Process_Decryption(dto.Window_height);
                    dto.Window_width = AES.Process_Decryption(dto.Window_width);
                    dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                    dto.Password = AES.Process_Decryption(dto.Password);
                    dto.New_password = AES.Process_Decryption(dto.New_password);

                    if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                        Screen_extend = dto.Screen_extend,
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
                        Controller = "Selected",
                        Action = "Password"
                    }).Result)
                        return Conflict();

                    dto.End_User_ID = dto.JWT_id;

                    if (dto.Login_type.ToUpper() == "EMAIL") {
                        string? email_address = _UsersRepository.Read_User_Email_By_ID(dto.JWT_id).Result;

                        byte[]? usersdb_SavedPasswordHash = _UsersRepository.Read_User_Password_Hash_By_ID(dto.JWT_id).Result;
                        byte[]? given_PasswordHash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes($"{dto.Password}"), Encoding.UTF8.GetBytes($"{email_address}{_Constants.JWT_SECURITY_KEY}")).Result;

                        if (usersdb_SavedPasswordHash != null)
                            if (!_UsersRepository.Compare_Password_Byte_Arrays(usersdb_SavedPasswordHash, given_PasswordHash).Result)
                                return Unauthorized();

                        return await Task.FromResult(_UsersRepository.Update_End_User_Password(new Password_ChangeDTO
                        {
                            End_User_ID = dto.JWT_id,
                            Password = dto.Password,
                            New_password = dto.New_password,
                            Email_address = email_address ?? "error"
                        })).Result;

                    }
                return "error";
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Status")]
        public async Task<ActionResult<string>> ChangeUserSelectedStatus([FromBody] Selected_StatusDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
                dto.Online_status = AES.Process_Decryption(dto.Online_status);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Status"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Status(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Custom_Label")]
        public async Task<ActionResult<string>> Change_User_Selected_Custom_Label([FromBody] Selected_StatusDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
                dto.Custom_lbl = AES.Process_Decryption(dto.Custom_lbl);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Status"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;
                dto.Online_status = 5.ToString();

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Status(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Theme")]
        public async Task<ActionResult<string>> ChangeUserSelectedTheme([FromBody] Selected_ThemeDTO dto)
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

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.ID = AES.Process_Decryption(dto.ID);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Theme"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Selected_Theme(dto).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Card_Border_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_BORDER_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
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
                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                
                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                
                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.Card_Border_Color = AES.Process_Decryption(dto.Card_Border_Color);
                
                dto.ID = AES.Process_Decryption(dto.ID);

                
                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Selected",
                    Action = "Card_Border_Color"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await Task.FromResult(_UsersRepository.Update_End_User_Card_Border_Color(dto).Result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Card_Header_Font")]
        public async Task<ActionResult<string>> UPDATE_CARD_HEADER_FONT([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Header_Font = AES.Process_Decryption(dto.Card_Header_Font);

            dto.ID = AES.Process_Decryption(dto.ID);

            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Header_Font"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Header_Font(dto).Result);
        }

        [HttpPut("Card_Header_Background_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_HEADER_BACKGROUND_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Header_Background_Color = AES.Process_Decryption(dto.Card_Header_Background_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Header_Background_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Header_Background_Color(dto).Result);
        }

        [HttpPut("Card_Header_Font_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_HEADER_FONT_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Header_Font_Color = AES.Process_Decryption(dto.Card_Header_Font_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Header_Font_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Header_Font_Color(dto).Result);
        }

        [HttpPut("Card_Body_Font")]
        public async Task<ActionResult<string>> UPDATE_CARD_BODY_FONT([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Body_Font = AES.Process_Decryption(dto.Card_Body_Font);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Body_Font"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Body_Font(dto).Result);
        }

        [HttpPut("Card_Body_Background_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_BODY_BACKGROUND_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Body_Background_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Body_Background_Color(dto).Result);
        }

        [HttpPut("Card_Body_Font_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_BODY_FONT_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Body_Font_Color = AES.Process_Decryption(dto.Card_Body_Font_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Body_Font_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Body_Font_Color(dto).Result);
        }

        [HttpPut("Card_Footer_Font")]
        public async Task<ActionResult<string>> UPDATE_CARD_FOOTER_FONT([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Footer_Font = AES.Process_Decryption(dto.Card_Footer_Font);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Footer_Font"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Footer_Font(dto).Result);
        }

        [HttpPut("Card_Footer_Background_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_FOOTER_BACKGROUND_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Footer_Background_Color = AES.Process_Decryption(dto.Card_Footer_Background_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Footer_Background_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Footer_Background_Color(dto).Result);
        }

        [HttpPut("Card_Footer_Font_Color")]
        public async Task<ActionResult<string>> UPDATE_CARD_FOOTER_FONT_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Card_Footer_Font_Color = AES.Process_Decryption(dto.Card_Footer_Font_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Card_Footer_Font_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Card_Footer_Font_Color(dto).Result);
        }

        [HttpPut("Navigation_Menu_Background_Color")]
        public async Task<ActionResult<string>> UPDATE_NAVIGATION_MENU_BACKGROUND_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Navigation_Menu_Background_Color = AES.Process_Decryption(dto.Navigation_Menu_Background_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Navigation_Menu_Background_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Navigation_Menu_Background_Color(dto).Result);
        }

        [HttpPut("Navigation_Menu_Font_Color")]
        public async Task<ActionResult<string>> UPDATE_NAVIGATION_MENU_FONT_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Navigation_Menu_Font_Color = AES.Process_Decryption(dto.Navigation_Menu_Font_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Navigation_Menu_Font_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Navigation_Menu_Font_Color(dto).Result);
        }

        [HttpPut("Navigation_Menu_Font")]
        public async Task<ActionResult<string>> UPDATE_NAVIGATION_MENU_FONT([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Navigation_Menu_Font = AES.Process_Decryption(dto.Navigation_Menu_Font);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Navigation_Menu_Font"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Navigation_Menu_Font(dto).Result);
        }

        [HttpPut("Button_Background_Color")]
        public async Task<ActionResult<string>> UPDATE_BUTTON_BACKGROUND_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Button_Background_Color = AES.Process_Decryption(dto.Button_Background_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Button_Background_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Button_Background_Color(dto).Result);
        }

        [HttpPut("Button_Font_Color")]
        public async Task<ActionResult<string>> UPDATE_BUTTON_FONT_COLOR([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            
            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

            
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

            
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

            
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Button_Font_Color = AES.Process_Decryption(dto.Button_Font_Color);

            
            dto.ID = AES.Process_Decryption(dto.ID);

            
            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Button_Font_Color"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Button_Font_Color(dto).Result);
        }

        [HttpPut("Button_Font")]
        public async Task<ActionResult<string>> UPDATE_BUTTON_FONT([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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
            dto.Button_Font = AES.Process_Decryption(dto.Button_Font);
            dto.ID = AES.Process_Decryption(dto.ID);

            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Button_Font"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Update_End_User_Button_Font(dto).Result);
        }

        [HttpPut("Theme_Default_Settings")]
        public async Task<ActionResult<string>> Update_All_Selected_Custom_Settings([FromBody] Selected_App_Custom_DesignDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
            dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
            dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
            dto.Language = AES.Process_Decryption(dto.Language);
            dto.Region = AES.Process_Decryption(dto.Region);
            dto.Location = AES.Process_Decryption(dto.Location);
            dto.Client_time = AES.Process_Decryption(dto.Client_time);
            dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
            dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;
            dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
            dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";
            dto.Window_height = AES.Process_Decryption(dto.Window_height);
            dto.Window_width = AES.Process_Decryption(dto.Window_width);
            dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

            if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
            {
                Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                Client_Networking_Port = HttpContext.Connection.RemotePort,
                Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Networking_Port = HttpContext.Connection.LocalPort,
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
                Screen_extend = dto.Screen_extend,
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
                Controller = "Selected",
                Action = "Theme_Default_Settings"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;
            return await Task.FromResult(_UsersRepository.Delete_End_User_Selected_App_Custom_Design(dto).Result);
        }

    }
}