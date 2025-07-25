﻿using Microsoft.AspNetCore.Mvc;

using System.Text;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using System.Text.Json;
using mpc_dotnetc_user_server.Controllers.Interfaces;
using mpc_dotnetc_user_server.Models.Interfaces;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<AuthenticateController> _logger;
        private static IConfiguration? _configuration;
        private readonly IUsers_Repository Users_Repository;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;
        private readonly IPassword Password;

        public AuthenticateController(
            ILogger<AuthenticateController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository,
            Constants constants,
            IAES aes,
            IJWT jwt,
            INetwork network,
            IPassword password
            )
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Network = network;
            Password = password;
        }

        [HttpPut("Login/Email")]
        public async Task<ActionResult<string>> Login_Login_Email_Address_And_Password([FromBody] Login_Email_PasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address).ToUpper();
                dto.Password = AES.Process_Decryption(dto.Password);

                dto.Locked = AES.Process_Decryption(dto.Locked);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.Grid_type = AES.Process_Decryption(dto.Grid_type);

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
                
                dto.Client_id = Users_Repository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;
                dto.JWT_id = dto.Client_id;

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

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_client_address = dto.JWT_client_address,
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
                    Controller = "Email",
                    Action = "Login"
                }).Result)
                    return Conflict();

                if (!Users_Repository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                {
                    await Users_Repository.Insert_Report_Failed_Unregistered_Email_Login_HistoryTbl(new Report_Failed_Unregistered_Email_Login_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Reason = "Unregistered Email",
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
                    return NotFound();
                }

                ulong user_id = Users_Repository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;
                    
                byte[]? user_password_hash_in_the_database = Users_Repository.Read_User_Password_Hash_By_ID(user_id).Result;
                byte[]? end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash = Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")).Result;

                if (user_password_hash_in_the_database != null) {
                    if (Password.Process_Comparison_Between_Password_Salted_Hash_Bytes(user_password_hash_in_the_database, end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash).Result) {
                        await Users_Repository.Insert_Report_Failed_Email_Login_HistoryTbl(new Report_Failed_Email_Login_HistoryDTO
                        {
                            Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                            Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                            Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                            Server_Port = HttpContext.Connection.LocalPort,
                            User_agent = dto.Server_user_agent,
                            Email_Address = dto.Email_Address,
                            Language = dto.Language,
                            Region = dto.Region,
                            Location = dto.Location,
                            Client_Time_Parsed = dto.Client_Time_Parsed,
                            Reason = "Incorrect Password",
                            End_User_ID = user_id,
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
                        return Unauthorized();
                    }
                }
                    
                byte end_user_selected_status = Users_Repository.Read_End_User_Selected_Status(new Selected_StatusDTO { 
                    End_User_ID = user_id                
                }).Result;

                if (end_user_selected_status == 0)
                {//User does not have a Status Record and will be set to Online Status.
                    await Users_Repository.Create_End_User_Status_Record(new Selected_StatusDTO
                    {
                        End_User_ID = user_id
                    });
                }

                if (end_user_selected_status != 1 && end_user_selected_status != 0)
                {//User has a Hidden Status Saved in the Database from Previous Login.
                    await Users_Repository.Update_End_User_Selected_Status(new Selected_StatusDTO
                    {
                        End_User_ID = user_id,
                        Online_status = 2.ToString()
                    });
                }

                await Users_Repository.Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO 
                { 
                    End_User_ID = user_id,
                    Alignment = dto.Alignment
                });

                await Users_Repository.Update_End_User_Selected_TextAlignment(new Selected_App_Text_AlignmentDTO
                {
                    End_User_ID = user_id,
                    Text_alignment = dto.Text_alignment
                });

                await Users_Repository.Update_End_User_Selected_Nav_Lock(new Selected_Navbar_LockDTO
                {
                    End_User_ID = user_id,
                    Locked = dto.Locked
                });

                await Users_Repository.Update_End_User_Selected_Language(new Selected_LanguageDTO
                {
                    End_User_ID = user_id,
                    Language = dto.Language,
                    Region = dto.Region
                });

                await Users_Repository.Update_End_User_Selected_Theme(new Selected_ThemeDTO
                {
                    End_User_ID = user_id,
                    Theme = dto.Theme
                });

                string user_data = await Task.FromResult(Users_Repository.Read_Email_User_Data_By_ID(user_id)).Result;
                var jsonDoc = JsonSerializer.Deserialize<Dictionary<string, string>>(user_data);
                string user_token = "";

                if (jsonDoc != null && jsonDoc.ContainsKey("token"))
                {
                    user_token = jsonDoc["token"];
                }

                await Users_Repository.Update_End_User_Login_Time_Stamp(new Login_Time_StampDTO
                {
                    End_User_ID = user_id,
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    User_agent = dto.Server_user_agent,
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
                    Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                    Token = user_token
                });

                await Users_Repository.Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_HistoryDTO
                {
                    End_User_ID = user_id,
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    User_agent = dto.Server_user_agent,
                    Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
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
                    Token = user_token
                });

                return user_data;

            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Logout")]
        public async Task<ActionResult<string>> Logout([FromBody] LogoutDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Locked = AES.Process_Decryption(dto.Locked);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.Grid_type = AES.Process_Decryption(dto.Grid_type);
                dto.Online_status = AES.Process_Decryption(dto.Online_status);

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

                if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Token = dto.Token,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
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
                    Controller = "Selected",
                    Action = "Status"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                switch (int.Parse(dto.Online_status)) {
                    case 1:
                        await Users_Repository.Update_End_User_Selected_Status(new Selected_StatusDTO { 
                            End_User_ID = dto.End_User_ID,
                            Online_status = 10.ToString()
                        });
                        break;
                    case 2:
                        await Users_Repository.Update_End_User_Selected_Status(new Selected_StatusDTO
                        {
                            End_User_ID = dto.End_User_ID,
                            Online_status = 20.ToString()
                        });
                        break;
                    case 3:
                        await Users_Repository.Update_End_User_Selected_Status(new Selected_StatusDTO
                        {
                            End_User_ID = dto.End_User_ID,
                            Online_status = 30.ToString()
                        });
                        break;
                    case 4:
                        await Users_Repository.Update_End_User_Selected_Status(new Selected_StatusDTO
                        {
                            End_User_ID = dto.End_User_ID,
                            Online_status = 40.ToString()
                        });
                        break;
                    case 5:
                        await Users_Repository.Update_End_User_Selected_Status(new Selected_StatusDTO
                        {
                            End_User_ID = dto.End_User_ID,
                            Online_status = 50.ToString()
                        });
                        break;
                }

                await Users_Repository.Insert_End_User_Logout_HistoryTbl(new Logout_Time_StampDTO {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    User_agent = dto.Server_user_agent,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    End_User_ID = dto.JWT_id,
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
                    Token = dto.Token,
                });

                await Users_Repository.Update_End_User_Logout(new Logout_Time_StampDTO {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    User_agent = dto.Server_user_agent,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                    End_User_ID = dto.JWT_id,
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
                    Token = dto.Token
                });

                return "Successfully Logged out.";
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}