namespace mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address
{
    public class Completed_Email_Account_CreationDTO
    {
        public string created_on { get; set; } = string.Empty;
        public string login_on { get; set; } = string.Empty;
        public string location { get; set; } = string.Empty;
        public string login_type { get; set; } = string.Empty;
        public byte account_type { get; set; }
        public string grid_type { get; set; } = string.Empty;
        public int online_status { get; set; }
        public ulong id { get; set; } 
        public string name { get; set; } = string.Empty;
        public string email_address { get; set; } = string.Empty;
        public string language { get; set; } = string.Empty;
        public string region { get; set; } = string.Empty;
        public byte alignment { get; set; } 
        public bool nav_lock { get; set; } 
        public byte text_alignment { get; set; }
        public byte theme { get; set; }
        public string roles { get; set; } = string.Empty;
        public string groups { get; set; } = string.Empty;
    }

}
