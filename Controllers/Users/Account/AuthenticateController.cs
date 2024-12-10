using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Authentication;

using System.Text;
using mpc_dotnetc_user_server.Models.Users.Selection;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public AuthenticateController(ILogger<AuthenticateController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPut("Login/Email")]
        public async Task<ActionResult<string>> Login_With_Login_Email_PasswordDTO([FromBody] Login_Email_PasswordDTO obj)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return BadRequest();
                
                if (!_UsersRepository.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address).Result)
                    return Conflict();

                ulong user_id = _UsersRepository.Read_User_ID_By_Email_Address(obj.Email_Address).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();

                byte[]? user_id_password_hash_in_the_database = _UsersRepository.Read_User_Password_Hash_By_ID(user_id).Result;
                byte[]? end_user_given_password_that_becomes_hash_to_compare_db_hash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{obj.Email_Address}MPCSalt")).Result;

                if (user_id_password_hash_in_the_database != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_id_password_hash_in_the_database, end_user_given_password_that_becomes_hash_to_compare_db_hash))
                        return Unauthorized();
                
                byte end_user_selected_status = _UsersRepository.Read_End_User_Selected_Status(new Selected_StatusDTO { 
                    User_id = user_id                
                }).Result;

                if (end_user_selected_status != 1)
                {//User has a Hidden Status Saved in the Database from Previous Login.
                    await _UsersRepository.Update_End_User_Selected_Status(new Selected_StatusDTO
                    {
                        User_id = user_id,
                        Online_status = 2
                    });
                }

                if (end_user_selected_status == 0)
                {//User does not have a Status Record and will be set to Online Status.
                    await _UsersRepository.Create_End_User_Status_Record(new Selected_StatusDTO
                    {
                        User_id = user_id
                    });
                }

                return await _UsersRepository.Update_End_User_Login(new Login_Time_StampDTO
                {
                    User_id = user_id
                });
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPut("Logout")]
        public async Task<ActionResult<string>> Logout([FromBody] Selected_StatusDTO dto)
        {
            try
            {
                ulong end_user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;
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