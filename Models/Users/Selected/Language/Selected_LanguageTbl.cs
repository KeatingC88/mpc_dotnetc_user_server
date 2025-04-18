﻿namespace mpc_dotnetc_user_server.Models.Users.Selected.Language
{
    public class Selected_LanguageTbl
    {
        public bool Deleted { get; set; }
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public string Language_code { get; set; } = string.Empty;
        public string Region_code { get; set; } = string.Empty;
    }
}
