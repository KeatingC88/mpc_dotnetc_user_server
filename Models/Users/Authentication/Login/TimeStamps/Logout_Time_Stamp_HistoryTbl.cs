namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps
{
    public class Logout_Time_Stamp_HistoryTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Logout_on { get; set; }
        public bool Deleted { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Created_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public string Location { get; set; } = string.Empty;
        public ulong Client_time { get; set; }
        public string Client_IP { get; set; } = string.Empty;
        public string Server_IP { get; set; } = string.Empty;
        public int Client_Port { get; set; }
        public int Server_Port { get; set; }
    }
}
