﻿using Microsoft.AspNetCore.Mvc;
using System.Text;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Selected.Deactivate;


namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Delete")]
    public class DeactivateUserController : ControllerBase
    {
        private readonly ILogger<DeactivateUserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        private readonly Constants _Constants;
        
        public DeactivateUserController(ILogger<DeactivateUserController> logger, IConfiguration configuration, IUsersRepository UsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
            _Constants = constants;
        }

        [HttpPost("User")]
        public async Task<ActionResult<string>> DeactivateUser([FromBody] Delete_UserDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Target_User = AES.Process_Decryption(@$"{dto.Target_User}");

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
                dto.Password = AES.Process_Decryption(dto.Password);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
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
                    Controller = "Delete",
                    Action = "User"
                }).Result)
                    return Conflict();

                string? email_address = _UsersRepository.Read_User_Email_By_ID(dto.JWT_id).Result;
                byte[]? user_password_hash_from_database_storage = _UsersRepository.Read_User_Password_Hash_By_ID(dto.JWT_id).Result;
                byte[]? user_password_given_from_end_user_on_gui_client = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{email_address}{_Constants.JWT_SECURITY_KEY}")).Result;
                if (user_password_hash_from_database_storage != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_password_hash_from_database_storage, user_password_given_from_end_user_on_gui_client).Result)
                        return Unauthorized();

                return await Task.FromResult(_UsersRepository.Delete_Account_By_User_id(new Delete_UserDTO { 
                    ID = dto.Client_id.ToString(),
                    Target_User = dto.Target_User
                }).Result);

            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}