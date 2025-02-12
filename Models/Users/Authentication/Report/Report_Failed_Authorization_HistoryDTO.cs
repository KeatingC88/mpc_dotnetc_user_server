﻿using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Report
{
    public class Report_Failed_Authorization_HistoryDTO
    {
        [Required]
        public ulong ID {  get; set; }
        [Required]
        public ulong Client_id { get; set; }
        [Required]
        public ulong JWT_id { get; set; }

        [Required(ErrorMessage = "Language is Missing.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region is Missing.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public ulong Client_time { get; set; }
        [Required]
        public string Client_Networking_IP_Address { get; set; } = string.Empty;
        [Required]
        public int Client_Networking_Port { get; set; }
        [Required]
        public string Server_Networking_IP_Address { get; set; }  = string.Empty;
        [Required]
        public string Controller { get; set; }  = string.Empty;
        [Required]
        public string Action { get; set; }  = string.Empty;
        [Required]
        public string Token { get; set; }  = string.Empty;
        [Required]
        public int Server_Networking_Port { get; set; }
        [Required]
        public string Server_User_Agent { get; set; } = string.Empty;
        [Required]
        public string Client_User_Agent { get; set; } = string.Empty;
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
        public string Screen_extend { get; set; } = string.Empty;
        [Required]
        public string Color_depth { get; set; } = string.Empty;
        [Required]
        public string Pixel_depth { get; set; } = string.Empty;
        [Required]
        public string Window_width { get; set; } = string.Empty;
        [Required]
        public string Window_height { get; set; } = string.Empty;
        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;
        [Required]
        public string JWT_client_key { get; set; } = string.Empty;
        [Required]
        public string JWT_client_address { get; set; } = string.Empty;
        public string User_agent { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public ulong User_id { get; set; }
    }
}


