namespace mpc_dotnetc_user_server.Models.Users.Integration
{
    public class Integration_TwitchDTO
    {
        public ulong ID { get; set; }
        public ulong End_User_ID { get; set; }
        public string Email_Address { get; set; } = string.Empty;
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Twitch_ID { get; set; }
    }
}