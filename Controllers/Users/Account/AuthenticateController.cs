using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Authenticate;

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
        public async Task<ActionResult<string>> Login_With_Email_Password([FromBody] Email_Password obj)//has it's own model for just here and has validation model
        {
            try
            {
                if (!ModelState.IsValid) 
                    return BadRequest();
                
                if (!_UsersRepository.Email_Exists_In_Login_Email_Address_Tbl(obj.Email_Address).Result)
                    return NotFound();

                ulong user_id = _UsersRepository.Get_User_ID_By_Email_Address(obj.Email_Address).Result;

                if (user_id == 0 || !_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                byte[]? user_id_password_hash_in_the_database = _UsersRepository.Get_User_Password_Hash_By_ID(user_id).Result;
                byte[]? end_user_given_password_that_becomes_hash_to_compare_db_hash = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{obj.Email_Address}MPCSalt")).Result;

                if (user_id_password_hash_in_the_database != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_id_password_hash_in_the_database, end_user_given_password_that_becomes_hash_to_compare_db_hash))
                        return Unauthorized();


                DTO dto = new DTO();//repository has it's own model with no validations for irepo
                dto.ID = user_id;
                dto.Password = obj.Password;
                dto.Email_Address = obj.Email_Address;

                byte end_user_selected_status = _UsersRepository.Read_End_User_Selected_Status(dto).Result;

                if (end_user_selected_status != 1)
                {//This is here incase user has selected "hidden" and they are signing into the server.
                    dto.Online_status = 2; // User came online.
                    await _UsersRepository.Update_User_Selected_Status(dto);//Update Database User Status Table.
                }

                return await _UsersRepository.Update_User_Login(dto);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//Login.

        [HttpPut("Logout")]
        public async Task<ActionResult<string>> Logout([FromBody] DTO obj)
        {
            try
            {
                ulong end_user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;
                if (end_user_id == 0 || !_UsersRepository.ID_Exists_In_Users_Tbl(end_user_id).Result)
                    return Unauthorized();
                
                obj.ID = end_user_id;
                switch (obj.Online_status) {//We capturing the user's last online-status for when they sign back in using that same status publicly to other end users.
                    case 1:
                        obj.Online_status = 10;//1-0 i.e. like save status as hidden-offline since end user is trying to logout.
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 2:
                        obj.Online_status = 20;//2-0 convention: (online value 2) - (offline value 0).
                        await _UsersRepository.Update_User_Selected_Status(obj);//status in obj is recorded into the database.
                        break;
                    case 3:
                        obj.Online_status = 30;//3-0 
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 4:
                        obj.Online_status = 40;//4-0 
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                    case 5:
                        obj.Online_status = 50;//5-0
                        await _UsersRepository.Update_User_Selected_Status(obj);
                        break;
                }//Locate End User's Submitted Status Protocol.

                return await _UsersRepository.Update_User_Logout(obj.ID);//return true if successful
            } catch (Exception e) {//Except
                return StatusCode(500, $"{e.Message}");//Error
            }//Try
        }//PUT End User into Logout State
    }//Controller.
}//Namespace.