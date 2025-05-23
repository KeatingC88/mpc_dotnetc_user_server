﻿
using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Controllers.Interfaces;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace mpc_dotnetc_user_server.Controllers.Services
{
    public class JWT : IJWT
    {
        private static readonly Constants Constants = new Constants();
        private static readonly ushort token_expire_time = 16;
        private readonly IAES AES;

        public JWT(IAES aes)
        {
            AES = aes;
        }

        public async Task<string> Create_Email_Account_Token(JWT_DTO dto)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Actor, $"{AES.Process_Encryption(dto.Account_type.ToString())}"),
                new Claim(ClaimTypes.NameIdentifier, $"{AES.Process_Encryption(dto.End_User_ID.ToString())}"),
                new Claim(ClaimTypes.Role, $"{AES.Process_Encryption($"{dto.User_roles}")}"),
                new Claim(ClaimTypes.GroupSid, $"{AES.Process_Encryption(dto.User_groups)}"),
                new Claim(ClaimTypes.Webpage, $"{AES.Process_Encryption(Constants.JWT_CLAIM_WEBPAGE)}"),
                new Claim(ClaimTypes.Email, $"{AES.Process_Encryption(dto.Email_address)}"),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Constants.JWT_SECURITY_KEY));

            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                $"{AES.Process_Encryption(Constants.JWT_ISSUER_KEY)}",
                $"{AES.Process_Encryption(Constants.JWT_CLIENT_KEY)}",
                claims,
                expires: DateTime.UtcNow.AddMinutes(token_expire_time),
                signingCredentials: signIn);

            return await Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<ulong> Read_Email_Account_User_ID_By_JWToken(string jwt_token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwt_token);
            List<object> values = jwtSecurityToken.Payload.Values.ToList();
            ulong currentTime = Convert.ToUInt64(((DateTimeOffset)DateTime.UtcNow).ToUnixTimeSeconds());
            ulong token_expire = Convert.ToUInt64(values[6]);

            bool tokenExpired = token_expire < currentTime ? true : false;

            if (tokenExpired)
                return 0;

            return await Task.FromResult(Convert.ToUInt64(AES.Process_Decryption($"{values[1].ToString()}")));
        }

        public async Task<ulong> Read_Email_Account_User_Role_By_JWToken(string jwt_token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = handler.ReadJwtToken(jwt_token);
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