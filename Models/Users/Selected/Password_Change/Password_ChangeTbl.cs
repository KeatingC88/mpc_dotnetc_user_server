namespace mpc_dotnetc_user_server.Models.Users.Selected.Password_Change
{
    public class Password_ChangeTbl
    {
        public long ID { get; set; }
        public long User_ID { get; set; }
        public byte[]? Password { get; set; }
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Updated_on { get; set; }
        public long Updated_by { get; set; }
    }
}
