namespace mpc_dotnetc_user_server.Models.Users.Selected.Avatar
{
    public class Selected_Avatar
    {
        public bool Deleted { get; set; }
        public long ID { get; set; }
        public long End_User_ID { get; set; }
        public long Updated_by { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public long Updated_on { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public string Avatar_title { get; set; } = string.Empty;
        public string Avatar_url_path { get; set; } = string.Empty;
    }
}
