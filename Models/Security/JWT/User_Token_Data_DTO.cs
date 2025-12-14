namespace mpc_dotnetc_user_server.Models.Security.JWT
{
    public class User_Token_Data_DTO
    {
        public long id { get; set; }
        public long? twitch_id { get; set; }
        public long? discord_id { get; set; }
        public byte account_type { get; set; }
        public string email_address { get; set; } = string.Empty;
        public string twitch_email_address { get; set; } = string.Empty;
        public string discord_email_address { get; set; } = string.Empty;
        public string groups { get; set; } = string.Empty;
        public string roles { get; set; } = string.Empty;
    }   
}
