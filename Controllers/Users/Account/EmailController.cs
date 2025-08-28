using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using System.Text;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IAES AES;
        private readonly INetwork Network;
        private readonly IValid Valid;
        private readonly IPassword Password;
        private readonly IJWT JWT;
        private readonly IWebHostEnvironment _env;
        public EmailController(
            ILogger<EmailController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository,
            IValid valid,
            IAES aes,
            IPassword password,
            INetwork network,
            IJWT jwt,
            Constants constants,
            IWebHostEnvironment env)
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            _Constants = constants;
            AES = aes;
            Network = network;
            Valid = valid;
            Password = password;
            JWT = jwt;
            _env = env;
        }

        [HttpPut("Login")]
        public async Task<ActionResult<string>> Login_Email_Address_And_Password([FromBody] Login_Twitchdto dto)
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

                if (user_password_hash_in_the_database != null)
                {
                    if (Password.Process_Comparison_Between_Password_Salted_Hash_Bytes(user_password_hash_in_the_database, end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash).Result)
                    {
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

                byte end_user_selected_status = Users_Repository.Read_End_User_Selected_Status(new Selected_StatusDTO
                {
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

                User_Data_DTO user_data = await Task.FromResult(Users_Repository.Read_User_Data_By_ID(user_id)).Result;

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
                    Client_Time_Parsed = dto.Client_Time_Parsed
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
                    Device_ram_gb = dto.Device_ram_gb
                });

                string created_email_account_token = JWT.Create_Email_Account_Token(new JWT_DTO
                {
                    End_User_ID = user_data.id,
                    User_groups = user_data.groups,
                    User_roles = user_data.roles,
                    Account_type = user_data.account_type,
                    Email_address = dto.Email_Address
                }).Result;

                CookieOptions cookie_options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddMinutes(_Constants.JWT_EXPIRE_TIME)
                };

                HttpContext.Session.SetString($@"AUTH|MPC:{user_data.id.ToString()}|EMAIL_ADDRESS:{user_data.email_address}", JsonSerializer.Serialize(user_data));
                Response.Cookies.Append(@$"{Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME")}", created_email_account_token, cookie_options);

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    user_data
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Confirmation")]
        public async Task<ActionResult<string>> Validating_Email_Address_User_Information_Before_Submission([FromBody] Confirmation_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address).ToUpper();
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
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
                    Controller = "Email",
                    Action = "Register"
                }).Result)
                    return Conflict();

                if (!Valid.Email(dto.Email_Address))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Controller = "Email",
                        Action = "Confirmation",
                        Reason = "Invalid Email Address",
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
                    return BadRequest();
                }

                if (!Valid.Language_Code(dto.Language))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Controller = "Email",
                        Action = "Confirmation",
                        Reason = "Invalid Language Code",
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
                    return BadRequest();
                }

                if (!Valid.Region_Code(dto.Region))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Controller = "Email",
                        Action = "Confirmation",
                        Reason = "Invalid Region Code",
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
                    return BadRequest();
                }

                if (!Users_Repository.Email_Exists_In_Pending_Email_RegistrationTbl(dto.Email_Address).Result)
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Reason = "JWT Mismatch",
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
                    return BadRequest();
                }

                if (Users_Repository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Controller = "Email",
                        Action = "Confirmation",
                        Reason = "Email Already Registered",
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
                    return BadRequest();
                }

                return Ok();
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Exists")]
        public async Task<ActionResult<string>> Validating_Email_Exists_In_Login_Email_Address_Tbl([FromBody] Validate_Email_AddressDTO dto) 
        {
            try {

                if (!ModelState.IsValid)
                    BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address).ToUpper();

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
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
                    Login_type = "Email",
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
                    Controller = "Email",
                    Action = "Exists"
                }).Result)
                    return Conflict();

                if (!Valid.Email(dto.Email_Address))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Exists",
                        Controller = "Email",
                        Reason = "Invalid Email Address",
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
                    return BadRequest();
                }

                if (!Valid.Language_Code(dto.Language))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Exists",
                        Controller = "Email",
                        Reason = "Invalid Language Code",
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
                    return BadRequest();
                }

                if (!Valid.Region_Code(dto.Region))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Exists",
                        Controller = "Email",
                        Reason = "Invalid Region Code",
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
                    return BadRequest();
                }

                if (Users_Repository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address.ToUpper()).Result ||
                    Users_Repository.Email_Exists_In_Twitch_Email_AddressTbl(dto.Email_Address.ToUpper()).Result)
                {
                    ulong user_id = Users_Repository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;

                    await Users_Repository.Insert_Report_Email_RegistrationTbl(new Report_Email_RegistrationDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        End_User_ID = user_id,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Reason = "Email Already Registered",
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

                    return BadRequest();
                }

                return Ok();
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Registering_An_Email_Account_For_New_User([FromBody] Pending_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();
                
                dto.Email_Address = AES.Process_Decryption(dto.Email_Address).ToUpper();
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time.ToString()));
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
                    Controller = "Email",
                    Action = "Register"
                }).Result)
                    return Conflict();

                if (!Valid.Email(dto.Email_Address))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Register",
                        Controller = "Email",
                        Reason = "Invalid Email Address",
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
                    return BadRequest();
                }

                if (!Valid.Language_Code(dto.Language))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Register",
                        Controller = "Email",
                        Reason = "Invalid Language Code",
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
                    return BadRequest();
                }

                if (!Valid.Region_Code(dto.Region))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Register",
                        Controller = "Email",
                        Reason = "Invalid Region Code",
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
                    return BadRequest();
                }

                if (Users_Repository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Register",
                        Controller = "Email",
                        Reason = "Email Already Registered",
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
                    return BadRequest();
                }

                await Users_Repository.Insert_Pending_Email_Registration_History_Record(new Pending_Email_Registration_HistoryDTO
                {
                    Email_Address = dto.Email_Address,
                    Language = dto.Language,
                    Region = dto.Region,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Location = dto.Location,
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                    Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                    User_agent = dto.Server_user_agent,
                    Code = dto.Code,
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

                if (Users_Repository.Email_Exists_In_Pending_Email_RegistrationTbl(dto.Email_Address).Result)
                    return await Task.FromResult(Users_Repository.Update_Pending_Email_Registration_Record(dto)).Result;

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize(new
                {
                    mpc_data = Users_Repository.Create_Pending_Email_Registration_Record(new Pending_Email_RegistrationDTO
                    {
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Client_time = dto.Client_time,
                        Location = dto.Location,
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Server_user_agent,
                        Code = dto.Code,
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
                    }).Result
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Submit")]
        public async Task<ActionResult<string>> Email_Submit([FromBody] Submit_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address).ToUpper();
                dto.Name = AES.Process_Decryption(dto.Name);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Password = AES.Process_Decryption(dto.Password);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Nav_lock = AES.Process_Decryption(dto.Nav_lock);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.Grid_type = AES.Process_Decryption(dto.Grid_type);
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
                    Controller = "Email",
                    Action = "Submit"
                }).Result)
                    return Conflict();

                if (Users_Repository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result)
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Client_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Submit",
                        Controller = "Email",
                        Reason = "Email Already Registered in Login_Email_AddressTbl",
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
                    return BadRequest();
                }

                if (!Users_Repository.Email_Exists_In_Pending_Email_RegistrationTbl(dto.Email_Address).Result)
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Client_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Submit",
                        Controller = "Email",
                        Reason = "Email Not Found in Pending_Email_RegistrationTbl",
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
                    return BadRequest();
                }

                if (!Valid.Email(dto.Email_Address))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Client_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Submit",
                        Controller = "Email",
                        Reason = "Invalid Email Address",
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
                    return BadRequest();
                }
                if (!Valid.Password(dto.Password))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Client_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Submit",
                        Controller = "Email",
                        Reason = "Invalid Password",
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
                    return BadRequest();
                }
                if (!Valid.Language_Code(dto.Language))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Client_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Submit",
                        Controller = "Email",
                        Reason = "Invalid Language Code",
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
                    return BadRequest();
                }
                if (!Valid.Region_Code(dto.Region))
                {
                    await Users_Repository.Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(new Report_Failed_Pending_Email_Registration_HistoryDTO
                    {
                        Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                        Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                        Server_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Port = HttpContext.Connection.LocalPort,
                        Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                        Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                        User_agent = dto.Client_user_agent,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                        Action = "Submit",
                        Controller = "Email",
                        Reason = "Invalid Region Code",
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
                    return BadRequest();
                }

                Completed_Email_Account_CreationDTO account_creation_data = Users_Repository.Create_Account_By_Email(new Complete_Email_RegistrationDTO
                {
                    Email_Address = dto.Email_Address,
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
                    Password = dto.Password,
                    Grid_type = byte.Parse(dto.Grid_type),
                    Name = dto.Name,
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

                string created_email_account_token = JWT.Create_Email_Account_Token(new JWT_DTO
                {
                    End_User_ID = account_creation_data.id,
                    User_groups = account_creation_data.groups,
                    User_roles = account_creation_data.roles,
                    Account_type = account_creation_data.account_type,
                    Email_address = account_creation_data.email_address
                }).Result;

                CookieOptions cookie_options = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddMinutes(_Constants.JWT_EXPIRE_TIME)
                };

                HttpContext.Session.SetString($@"AUTH|MPC:{account_creation_data.id.ToString()}|EMAIL_ADDRESS:{account_creation_data.email_address}", JsonSerializer.Serialize(account_creation_data));
                Response.Cookies.Append(@$"{Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME")}", created_email_account_token, cookie_options);

                return await Task.FromResult(Ok(AES.Process_Encryption(JsonSerializer.Serialize( new {
                    account_creation_data
                }))));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}