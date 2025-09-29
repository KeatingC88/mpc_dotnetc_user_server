namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email
{
    public class User_Data_DTO
    {
        public long id { get; set; }
        public long twitch_id { get; set; }
        public long discord_id { get; set; }
        public byte account_type { get; set; }
        public string email_address { get; set; } = string.Empty;
        public string twitch_email_address { get; set; } = string.Empty;
        public string twitch_user_name { get; set; } = string.Empty;
        public string discord_email_address { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public long login_on { get; set; }
        public long logout_on { get; set; }
        public string current_language { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public string region { get; set; } = string.Empty;
        public byte online_status { get; set; }
        public string custom_lbl { get; set; } = string.Empty;
        public long created_on { get; set; }
        public string avatar_url_path { get; set; } = string.Empty;
        public string avatar_title { get; set; } = string.Empty;
        public string login_type { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public byte theme { get; set; }
        public byte alignment { get; set; }
        public byte text_alignment { get; set; }
        public byte gender { get; set; }
        public byte birth_day { get; set; }
        public byte birth_month { get; set; }
        public long birth_year { get; set; }
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string middle_name { get; set; } = string.Empty;
        public string maiden_name { get; set; } = string.Empty;
        public string ethnicity { get; set; } = string.Empty;
        public string groups { get; set; } = string.Empty;
        public string roles { get; set; } = string.Empty;
        public byte grid_type { get; set; }
        public bool nav_lock { get; set; }
        public string card_border_color { get; set; } = string.Empty;
        public string card_header_font { get; set; } = string.Empty;
        public string card_header_font_color { get; set; } = string.Empty;
        public string card_header_background_color { get; set; } = string.Empty;
        public string card_body_font { get; set; } = string.Empty;
        public string card_body_background_color { get; set; } = string.Empty;
        public string card_body_font_color { get; set; } = string.Empty;
        public string card_footer_font_color { get; set; } = string.Empty;
        public string card_footer_font { get; set; } = string.Empty;
        public string card_footer_background_color { get; set; } = string.Empty;
        public string navigation_menu_background_color { get; set; } = string.Empty;
        public string navigation_menu_font_color { get; set; } = string.Empty;
        public string navigation_menu_font { get; set; } = string.Empty;
        public string button_background_color { get; set; } = string.Empty;
        public string button_font { get; set; } = string.Empty;
        public string button_font_color { get; set; } = string.Empty;

    }
}
