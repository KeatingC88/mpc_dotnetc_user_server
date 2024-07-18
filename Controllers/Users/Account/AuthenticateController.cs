using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users;
using System.Text;

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
        public async Task<ActionResult<string>> LoginEmailPassword([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || string.IsNullOrWhiteSpace(obj.Email_address) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password))
                    return BadRequest();

                if (!_UsersRepository.Email_Exists_In_Login_Email_Address_Tbl(obj.Email_address).Result)
                    return NotFound();

                ulong user_id = _UsersRepository.Get_User_ID_By_Email_Address(obj.Email_address).Result;

                if (user_id == 0 || !_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                byte[]? user_id_password_hash_in_the_database = _UsersRepository.Get_User_Password_Hash_By_ID(user_id).Result;
                byte[]? end_user_given_password_that_becomes_hash_to_compare_db_hash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{obj.Email_address}MPCSalt")).Result;

                if (user_id_password_hash_in_the_database != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_id_password_hash_in_the_database, end_user_given_password_that_becomes_hash_to_compare_db_hash))
                        return Unauthorized();

                obj.ID = user_id;

                byte end_user_selected_status = _UsersRepository.Read_End_User_Selected_Status(obj).Result;
                if (end_user_selected_status != 1)
                {//This is here incase user has selected "hidden" and they are signing into the server.
                    obj.Online_status = 2;//User came online.
                    await _UsersRepository.Update_User_Selected_Status(obj);//Update Database Status Table.
                }

                return await Task.FromResult(_UsersRepository.Update_User_Login(obj)).Result;//return timestamp and id
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//Login.

        [HttpPut("Logout")]
        public async Task<ActionResult<bool>> Logout([FromBody] DTO obj)
        {
            try
            {
                ulong end_user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;
                if (end_user_id == 0 || !_UsersRepository.ID_Exists_In_Users_Tbl(end_user_id).Result)
                    return Unauthorized();
                
                obj.ID = end_user_id;
                switch (obj.Online_status) {
                    case 1:
                        obj.Online_status = 10;//1-0 i.e. like save status as hidden-offline since end user is trying to logout.
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 2:
                        obj.Online_status = 20;//2-0 (online-offline status saved)
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 3:
                        obj.Online_status = 30;//3-0 (away-offline status saved)
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 4:
                        obj.Online_status = 40;//2-0 (dnd-offline status saved)
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 5:
                        obj.Online_status = 50;//2-0 (custom-offline status saved)
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                }//Locate End User's Submitted Status Protocol.

                return await Task.FromResult(_UsersRepository.Update_User_Logout(obj.ID).Result);//return true if successful
            } catch (Exception e) {//Except
                return StatusCode(500, $"{e.Message}");//Error
            }//Try
        }//PUT End User into Logout State
    }//Controller.
}//Namespace.