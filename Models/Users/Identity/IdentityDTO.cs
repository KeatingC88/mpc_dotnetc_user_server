using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Identity
{
    public class IdentityDTO
    {
        [Required]
        public ulong End_User_ID { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Middle_name { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Maiden_name { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public string Month { get; set; }
        public string Day { get; set; }
        public string Year { get; set; }

        [Required]
        public string ID { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Client_time { get; set; } = string.Empty;          public ulong Client_Time_Parsed { get; set; } 

        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

        [Required]
        public string Account_type { get; set; } = string.Empty;

        [Required]
        public string Login_type { get; set; } = string.Empty;

        public string Server_user_agent { get; set; } = string.Empty;

        [Required]
        public string User_agent { get; set; } = string.Empty;

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

        public ulong Client_id { get; set; }

        public ulong JWT_id { get; set; }

        public string Client_user_agent { get; set; } = string.Empty;
    }
}