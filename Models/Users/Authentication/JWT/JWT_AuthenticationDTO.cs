﻿using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.JWT
{
    public class JWT_AuthenticationDTO
    {
        [Required]
        public string Language { get; set; } = string.Empty;
        [Required]
        public string Region { get; set; } = string.Empty;
        [Required]
        public string Location { get; set; } = string.Empty;
        [Required]
        public string Login_type { get; set; } = string.Empty;
        [Required]
        public string Client_time { get; set; } = string.Empty;          public ulong Client_Time_Parsed { get; set; } 
        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;
        [Required]
        public string JWT_client_key { get; set; } = string.Empty;
        [Required]
        public string JWT_client_address { get; set; } = string.Empty;
        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Server_IP_Address { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int Server_Port { get; set; }
        public ulong Client_id { get; set; }
        public ulong JWT_id { get; set; }
        public ulong End_User_ID { get; set; }
        public string Email_Address { get; set; } = string.Empty;
    }
}