namespace mpc_dotnetc_user_server.Models.Users.Profile
{
    public class Profile_PageTbl
    {
        public long ID { get; set; }
        public long End_User_ID { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Updated_on { get; set; }
        public long Updated_by { get; set; }
        public string? Page_Title { get; set; } = string.Empty;
        public string? Page_Description { get; set; } = string.Empty;
        public string? About_Me { get; set; } = string.Empty;
        public string? Banner_URL { get; set; } = string.Empty;

    }
}