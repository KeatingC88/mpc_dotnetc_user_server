
using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Report 
{ 
    public class Report_Failed_JWT_HistoryTbl
    {        
        public string Language_Region { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;       
        public string Client_time { get; set; } = string.Empty;
        public string Login_type { get; set; } = string.Empty;
        public string JWT_issuer_key { get; set; } = string.Empty;
        public string JWT_client_key { get; set; } = string.Empty;
        public string JWT_client_address { get; set; } = string.Empty;
        public string Client_IP { get; set; } = string.Empty;
        public int Client_Port { get; set; }
        public string Server_IP { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int Server_Port { get; set; }
        public ulong Client_id { get; set; }
        public ulong JWT_id { get; set; }
        public ulong User_id { get; set; }
        public ulong ID { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Created_by { get; set; }
        [Required]
        public string User_agent { get; set; } = string.Empty;
        [Required]

        public string Client_user_Agent { get; set; } = string.Empty;
        [Required]

        public string Server_user_Agent { get; set; } = string.Empty;
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

    }
}