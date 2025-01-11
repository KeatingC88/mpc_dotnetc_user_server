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
using mpc_dotnetc_user_server.Models.Users.Authentication.Reported;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogger<AuthenticateController> _logger;
        private static IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public AuthenticateController(ILogger<AuthenticateController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }
        AES AES = new AES();

        [HttpPut("Login/Email")]
        public async Task<ActionResult<string>> Login_Login_Email_Address_And_Password([FromBody] Login_Email_PasswordDTO dto)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address);
                dto.Password = AES.Process_Decryption(dto.Password);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region); 
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Locked = AES.Process_Decryption(dto.Locked);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);

                if (!_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(dto.Email_Address).Result) 
                {
                    await _UsersRepository.Insert_Reported_Unregistered_EmailTbl(new Reported_Unregistered_EmailDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Email_Address = dto.Email_Address,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time)
                    });
                    return Conflict();
                }
                
                ulong user_id = _UsersRepository.Read_User_ID_By_Email_Address(dto.Email_Address).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();
                    
                byte[]? user_password_hash_in_the_database = _UsersRepository.Read_User_Password_Hash_By_ID(user_id).Result;
                byte[]? end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}MPCSalt")).Result;

                if (user_password_hash_in_the_database != null) {
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_password_hash_in_the_database, end_user_given_password_that_becomes_hash_given_to_compare_with_db_hash)) {
                        await _UsersRepository.Insert_Reported_Failed_Email_Login_HistoryTbl(new Reported_Failed_Email_Login_HistoryDTO
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
                            Reason = "Incorrect Password",
                            User_id = user_id
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
                    User_id = user_id
                });

                await _UsersRepository.Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO 
                { 
                    User_id = user_id,
                    Alignment = byte.Parse(dto.Alignment)
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
                    Theme = byte.Parse(dto.Theme)
                });

                return await Task.FromResult(_UsersRepository.Read_User(user_id)).Result;

            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Logout")]
        public async Task<ActionResult<string>> Logout([FromBody] Selected_StatusDTO dto)
        {
            try
            {
                ulong end_user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

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