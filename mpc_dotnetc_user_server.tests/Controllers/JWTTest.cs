using FluentAssertions;
using mpc_dotnetc_user_server.Controllers;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace mpc_dotnetc_user_server.tests.Controllers
{
    public class JWTTest
    {
        private readonly IAES AES;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public JWTTest()
        {
            _key = new byte[32];
            _iv = new byte[16];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(_key);
            rng.GetBytes(_iv);

            AES = new AES(_key, _iv);
        }

        [Fact]
        public async Task Create_Email_Account_Token_Should_Return_Valid_JWT()
        {
/*            // Arrange
            var service = new JWT(AES); // replace with your actual class name
            var dto = new JWT_DTO
            {
                Account_type = 1,
                End_User_ID = 123,
                User_roles = "Admin",
                User_groups = "GroupA",
                Email_address = "test@example.com"
            };

            // Act
            var token = await service.Create_Email_Account_Token(dto);

            // Assert
            token.Should().NotBeNullOrWhiteSpace("a JWT token should be returned");

            Action act = () => new JwtSecurityTokenHandler().ReadJwtToken(token);
            act.Should().NotThrow("the token should be a valid JWT format");

            var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
            jwt.Should().NotBeNull("a readable JWT should have been created");*/
        }

    }
}
