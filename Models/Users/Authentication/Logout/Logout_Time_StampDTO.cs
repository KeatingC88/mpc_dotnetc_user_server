﻿using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Logout
{
    public class Logout_Time_StampDTO
    {
        public ulong End_User_ID { get; set; }

        [Required(ErrorMessage = "Application Logout Time is Missing.")]
        public ulong Logout_on { get; set; }

        [Required(ErrorMessage = "Application Client Time is Missing.")]
        public string Client_time { get; set; } = string.Empty;

        public ulong Client_Time_Parsed { get; set; }

        [Required(ErrorMessage = "Application Logout Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        public string Remote_IP { get; set; } = string.Empty;
        public int Remote_Port { get; set; }
        public string Server_IP_Address { get; set; } = string.Empty;
        public string Client_IP { get; set; } = string.Empty;
        public int Server_Port { get; set; }
        public int Client_Port { get; set; }

        [Required]
        public string ID { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Alignment { get; set; } = string.Empty;

        [Required]
        public string Text_alignment { get; set; } = string.Empty;

        [Required]
        public string Locked { get; set; } = string.Empty;

        [Required]
        public string Grid_type { get; set; } = string.Empty;

        [Required]
        public string Theme { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Online_status { get; set; } = string.Empty;

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

        public ulong Client_id { get; set; }
        public ulong JWT_id { get; set; }

        [Required]
        public string User_agent { get; set; } = string.Empty;

        public string Client_user_agent { get; set; } = string.Empty;
        public string Server_user_agent { get; set; } = string.Empty;

        [Required]
        public string Down_link { get; set; } = string.Empty;

        [Required]
        public string Connection_type { get; set; } = string.Empty;

        [Required]
        public string RTT { get; set; } = string.Empty;

        [Required]
        public string Data_saver { get; set; } = string.Empty;

        [Required]
        public string Device_ram_gb { get; set; } = string.Empty;

        [Required]
        public string Orientation { get; set; } = string.Empty;

        [Required]
        public string Screen_width { get; set; } = string.Empty;

        [Required]
        public string Screen_height { get; set; } = string.Empty;


        [Required]
        public string Color_depth { get; set; } = string.Empty;

        [Required]
        public string Pixel_depth { get; set; } = string.Empty;

        [Required]
        public string Window_width { get; set; } = string.Empty;

        [Required]
        public string Window_height { get; set; } = string.Empty;
    }
}
