using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_App_Custom_DesignDTO
    {
        [Required]
        public string ID { get; set; } = string.Empty;
        public long End_User_ID { get; set; }
        public long Updated_by { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public string Card_Border_Color { get; set; } = string.Empty;
        public string Card_Header_Font { get; set; } = string.Empty;
        public string Card_Header_Background_Color { get; set; } = string.Empty;
        public string Card_Header_Font_Color { get; set; } = string.Empty;
        public string Card_Body_Font { get; set; } = string.Empty;
        public string Card_Body_Background_Color { get; set; } = string.Empty;
        public string Card_Body_Font_Color { get; set; } = string.Empty;
        public string Card_Footer_Font { get; set; } = string.Empty;
        public string Card_Footer_Background_Color { get; set; } = string.Empty;
        public string Card_Footer_Font_Color { get; set; } = string.Empty;
        public string Navigation_Menu_Background_Color { get; set; } = string.Empty;
        public string Navigation_Menu_Font_Color { get; set; } = string.Empty;
        public string Navigation_Menu_Font { get; set; } = string.Empty;
        public string Button_Background_Color { get; set; } = string.Empty;
        public string Button_Font_Color { get; set; } = string.Empty;
        public string Button_Font { get; set; } = string.Empty;

        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Client_time { get; set; } = string.Empty;          public long Client_Time_Parsed { get; set; } 

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

        public long Client_id { get; set; }
        public long JWT_id { get; set; }

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
