using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;

using System.Text;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.Authentication.Report;
using System.Security.Claims;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<AuthenticateController> _logger;
        private static IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public AuthenticateController(ILogger<AuthenticateController> logger, IConfiguration configuration, IUsersRepository UsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
            _Constants = constants;
        }

        

        [HttpPut("Login/Email")]
        public async Task<ActionResult<string>> Login_Login_Email_Address_And_Password([FromBody] Login_Email_PasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address);
                dto.Password = AES.Process_Decryption(dto.Password);
                dto.Locked = AES.Process_Decryption(dto.Locked);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                
                dto.Client_id = _UsersRepository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;
                dto.JWT_id = dto.Client_id;

                dto.User_agent = AES.Process_Decryption(dto.User_agent);
                var user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

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

                if (user_agent == "error" || dto.User_agent != user_agent)
                {
                    await _UsersRepository.Insert_Report_Failed_User_Agent_HistoryTbl(new Report_Failed_User_Agent_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "User-Agent Client-Server Mismatch",
                        Controller = "Authenticate",
                        Action = "Login_Email",
                        Login_type = "Email",
                        Server_User_Agent = user_agent,
                        Client_User_Agent = dto.User_agent,
                        User_id = dto.Client_id,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return Conflict();
                }

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
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-Server Mismatch",
                        Controller = "Authenticate",
                        Action = "Login_Email",
                        User_id = dto.JWT_id,
                        Login_type = "Email",
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return Conflict();
                }

                if (dto.Client_id != dto.JWT_id) 
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-ID Mismatch",
                        Controller = "Authenticate",
                        Action = "Login_Email",
                        User_id = dto.JWT_id,
                        Login_type = "Email",
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return Conflict();
                }

                if (!_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_Unregistered_Email_Login_HistoryTbl(new Report_Failed_Unregistered_Email_Login_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "Unregistered Email",
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return NotFound();
                }

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(dto.Client_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_Unregistered_Email_Login_HistoryTbl(new Report_Failed_Unregistered_Email_Login_HistoryDTO
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
                        Reason = "Unregistered Email ID",
                        User_agent = user_agent,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return NotFound();
                }

                ulong user_id = _UsersRepository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;
                    
                byte[]? user_password_hash_in_the_database = _UsersRepository.Read_User_Password_Hash_By_ID(user_id).Result;
                byte[]? end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}")).Result;

                if (user_password_hash_in_the_database != null) {
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_password_hash_in_the_database, end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash)) {
                        await _UsersRepository.Insert_Report_Failed_Email_Login_HistoryTbl(new Report_Failed_Email_Login_HistoryDTO
                        {
                            Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                            Client_Networking_Port = HttpContext.Connection.RemotePort,
                            Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                            Server_Networking_Port = HttpContext.Connection.LocalPort,
                            User_agent = user_agent,
                            Email_Address = dto.Email_Address,
                            Language = dto.Language,
                            Region = dto.Region,
                            Location = dto.Location,
                            Client_time = ulong.Parse(dto.Client_time),
                            Reason = "Incorrect Password",
                            User_id = user_id,
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
                            Device_ram_gb = dto.Device_ram_gb
                        });
                        return Unauthorized();
                    }
                }
                    
                byte end_user_selected_status = _UsersRepository.Read_End_User_Selected_Status(new Selected_StatusDTO { 
                    User_id = user_id                
                }).Result;

                if (end_user_selected_status == 0)
                {//User does not have a Status Record and will be set to Online Status.
                    await _UsersRepository.Create_End_User_Status_Record(new Selected_StatusDTO
                    {
                        User_id = user_id
                    });
                }

                if (end_user_selected_status != 1 && end_user_selected_status != 0)
                {//User has a Hidden Status Saved in the Database from Previous Login.
                    await _UsersRepository.Update_End_User_Selected_Status(new Selected_StatusDTO
                    {
                        User_id = user_id,
                        Online_status = 2
                    });
                }

                await _UsersRepository.Update_End_User_Login_Time_Stamp(new Login_Time_StampDTO
                {
                    User_id = user_id,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    User_agent = user_agent,
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
                    Location = dto.Location,
                    Client_time = ulong.Parse(dto.Client_time)
                });

                await _UsersRepository.Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_HistoryDTO {
                    User_id = user_id,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    User_agent = user_agent,
                    Location = dto.Location,
                    Client_time = ulong.Parse(dto.Client_time),
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
                    Device_ram_gb = dto.Device_ram_gb
                });

                await _UsersRepository.Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO 
                { 
                    User_id = user_id,
                    Alignment = dto.Alignment
                });

                await _UsersRepository.Update_End_User_Selected_TextAlignment(new Selected_App_Text_AlignmentDTO
                {
                    User_id = user_id,
                    Text_alignment = byte.Parse(dto.Text_alignment)
                });

                await _UsersRepository.Update_End_User_Selected_Nav_Lock(new Selected_Navbar_LockDTO
                {
                    User_id = user_id,
                    Locked = bool.Parse(dto.Locked)
                });

                await _UsersRepository.Update_End_User_Selected_Language(new Selected_LanguageDTO
                {
                    User_id = user_id,
                    Language = dto.Language,
                    Region = dto.Region
                });

                await _UsersRepository.Update_End_User_Selected_Theme(new Selected_ThemeDTO
                {
                    User_id = user_id,
                    Theme = dto.Theme
                });

                return await Task.FromResult(_UsersRepository.Read_Email_User_Data_By_ID(user_id)).Result;

            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Logout")]
        public async Task<ActionResult<string>> Logout([FromBody] Selected_StatusDTO dto)
        {
            try
            {
                ulong end_user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(end_user_id).Result)
                    return Unauthorized();
                
                dto.User_id = end_user_id;

                switch (dto.Online_status) {//We capturing the user's last online-status for when they sign back in using that same status publicly to other end users.
                    case 1:
                        dto.Online_status = 10;//1-0 i.e. like save status as hidden-offline since end user is trying to logout.
                        await _UsersRepository.Update_End_User_Selected_Status(dto);
                        break;
                    case 2:
                        dto.Online_status = 20;//2-0 convention: (online value 2) - (offline value 0).
                        await _UsersRepository.Update_End_User_Selected_Status(dto);//status in dto is recorded into the database.
                        break;
                    case 3:
                        dto.Online_status = 30;//3-0 
                        await _UsersRepository.Update_End_User_Selected_Status(dto);
                        break;
                    case 4:
                        dto.Online_status = 40;//4-0 
                        await _UsersRepository.Update_End_User_Selected_Status(dto);
                        break;
                    case 5:
                        dto.Online_status = 50;//5-0
                        await _UsersRepository.Update_End_User_Selected_Status(dto);
                        break;
                }

                return await _UsersRepository.Update_End_User_Logout(dto.User_id);//return true if successful
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}