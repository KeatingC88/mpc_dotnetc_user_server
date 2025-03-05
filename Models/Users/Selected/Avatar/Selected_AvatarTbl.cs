namespace mpc_dotnetc_user_server.Models.Users.Selected.Avatar
{
    public class Selected_AvatarTbl
    {
        public bool Deleted { get; set; }
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public string Avatar_title { get; set; } = string.Empty;
        public string Avatar_url_path { get; set; } = string.Empty;
    }
}
