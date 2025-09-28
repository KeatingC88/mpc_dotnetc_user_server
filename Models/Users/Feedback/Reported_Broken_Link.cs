namespace mpc_dotnetc_user_server.Models.Users.Feedback
{
    public class Reported_Broken_Link
    {
        public long ID { get; set; }
        public long End_User_ID { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Updated_on { get; set; }
        public long Updated_by { get; set; }
        public string URL { get; set; } = string.Empty;
    }
}
