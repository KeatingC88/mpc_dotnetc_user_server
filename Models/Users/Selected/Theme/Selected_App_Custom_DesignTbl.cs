namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_App_Custom_DesignTbl
    {
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
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
    }
}
