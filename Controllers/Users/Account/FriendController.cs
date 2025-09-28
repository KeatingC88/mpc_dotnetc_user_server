using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Report;
using mpc_dotnetc_user_server.Models.Users.Friends;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Friend")]
    public class FriendController : ControllerBase
    {
        private readonly ILogger<FriendController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;
        public FriendController(
            ILogger<FriendController> logger, 
            IConfiguration configuration, 
            IUsers_Repository users_repository,
            IJWT jwt,
            IAES aes,
            INetwork network
            )
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            JWT = jwt;
            AES = aes;
            Network = network;
        }
    }
}