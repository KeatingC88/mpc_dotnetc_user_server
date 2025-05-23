﻿using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;

namespace mpc_dotnetc_user_server.Controllers.Interfaces
{
    public interface IJWT
    {
        Task<string> Create_Email_Account_Token(JWT_DTO dto);
        Task<ulong> Read_Email_Account_User_ID_By_JWToken(string jwt_token);
        Task<ulong> Read_Email_Account_User_Role_By_JWToken(string jwt_token);
    }
}