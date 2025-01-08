namespace mpc_dotnetc_user_server.Models.Users._Index
{
    public class UserDTO
    {
        public ulong ID { get; set; }
        public ulong Created_on { get; set; }
        public ulong Created_by { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Target_User { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}
