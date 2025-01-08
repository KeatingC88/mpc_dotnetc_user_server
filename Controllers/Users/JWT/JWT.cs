using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace mpc_dotnetc_user_server.Controllers.Users.JWT
{
    public class JWT
    {
        private static readonly ushort token_expire_time = 2;
        private static AES AES = new AES();


        public static async Task<string> Create_Token(JWT_DTO dto)
        {
            List<Claim> claims = new List<Claim>
            {                
                new Claim(ClaimTypes.Actor, $"{AES.Process_Encryption(dto.account_type.ToString())}"),
                new Claim(ClaimTypes.NameIdentifier, $"{AES.Process_Encryption(dto.user_id.ToString())}"),
                new Claim(ClaimTypes.Role, $"{AES.Process_Encryption($"{dto.user_roles}")}"),
                new Claim(ClaimTypes.GroupSid, $"{AES.Process_Encryption(dto.user_groups)}"),
                new Claim(ClaimTypes.Webpage, $"{AES.Process_Encryption("http://localhost:6499")}")
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("9!5@a$59#%8^7MPC]1MPC999587)($@!53DataMonkey78912345645447890#%^2345vvcczxxedddg!#$%132577979798dA&*($##$$%@!^&*DFGGFFFFA^%YHBFSSDFTYG"));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                $"{AES.Process_Encryption("JWT-Authentication-MPC-User-Server-As-Issuer")}",
                $"{AES.Process_Encryption("JWT-Servicing-MPC-Client-As-Audience")}",
                claims,
                expires: DateTime.UtcNow.AddMinutes(token_expire_time),
                signingCredentials: signIn);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public static async Task<ulong> Read_User_ID_By_JWToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
            List<object> values = jwtSecurityToken.Payload.Values.ToList();
            ulong currentTime = Convert.ToUInt64(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            ulong token_expire = Convert.ToUInt64(values[2]);
            bool tokenExpired = token_expire < currentTime ? true : false;

            if (tokenExpired)
                return 0;

            return await Task.FromResult(Convert.ToUInt64(AES.Process_Decryption($"{values[0].ToString()}")));
        }

        public static async Task<ulong> Read_User_Role_By_JWToken(string jwtToken)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
            List<object> values = jwtSecurityToken.Payload.Values.ToList();
            ulong currentTime = Convert.ToUInt64(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            ulong token_expire = Convert.ToUInt64(values[2]);
            bool tokenExpired = token_expire < currentTime ? true : false;

            if (tokenExpired)
                return 0;

            return await Task.FromResult(Convert.ToUInt64(AES.Process_Decryption($"{values[1].ToString()}")));
        }
    }
}
