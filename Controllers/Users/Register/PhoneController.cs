using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Phone")]
    public class PhoneController : ControllerBase
    {
        private readonly ILogger<PhoneController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        public PhoneController(ILogger<PhoneController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPost("Confirmation")]
        public ActionResult<string> Phone_Confirmation([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Phone.ToString()) || 
                    string.IsNullOrWhiteSpace(obj.Phone.ToString()) ||
                    string.IsNullOrEmpty(obj.Code) || 
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    string.IsNullOrEmpty(obj.Carrier) || 
                    string.IsNullOrWhiteSpace(obj.Carrier) ||
                    string.IsNullOrEmpty(obj.Country.ToString()) || 
                    string.IsNullOrWhiteSpace(obj.Country.ToString()) ||
                    !Valid.Phone(obj.Phone) || _UsersRepository.Telephone_Exists_In_Login_Telephone_Tbl(obj.Phone).Result ||
                    !_UsersRepository.Phone_Exists_In_Telephone_Not_Confirmed_Tbl(obj.Phone).Result)
                    return BadRequest();

                return StatusCode(200, JsonSerializer.Serialize(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Phone_Register([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Phone.ToString()) ||
                    string.IsNullOrWhiteSpace(obj.Phone.ToString()) ||
                    string.IsNullOrEmpty(obj.Code) || 
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    string.IsNullOrEmpty(obj.Carrier) || 
                    string.IsNullOrWhiteSpace(obj.Carrier) ||
                    string.IsNullOrEmpty(obj.Country.ToString()) || 
                    string.IsNullOrWhiteSpace(obj.Country.ToString()) ||
                    !Valid.Phone(obj.Phone) ||
                    _UsersRepository.Telephone_Exists_In_Login_Telephone_Tbl(obj.Phone).Result)
                    return BadRequest();

                if (_UsersRepository.Phone_Exists_In_Telephone_Not_Confirmed_Tbl(obj.Phone).Result)
                    return await Task.FromResult(_UsersRepository.Update_Unconfirmed_Phone(obj).Result);

                return await Task.FromResult(_UsersRepository.Create_Unconfirmed_Phone(obj).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Submit")]
        public async Task<ActionResult<string>> PhoneSubmit([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Phone.ToString()) || string.IsNullOrWhiteSpace(obj.Phone.ToString()) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password) ||
                    string.IsNullOrEmpty(obj.Code) || string.IsNullOrWhiteSpace(obj.Code) ||
                    string.IsNullOrEmpty(obj.Carrier) || string.IsNullOrWhiteSpace(obj.Carrier) ||
                    string.IsNullOrEmpty(obj.Country.ToString()) || string.IsNullOrWhiteSpace(obj.Country.ToString()) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password) ||
                    !Valid.Phone(obj.Phone) || !Valid.Password(obj.Password) ||
                    !Valid.LanguageRegion(obj.Language))
                    return BadRequest();

                return await Task.FromResult(_UsersRepository.Create_Account_By_Phone(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.