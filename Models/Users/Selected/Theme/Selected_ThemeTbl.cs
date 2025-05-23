﻿namespace mpc_dotnetc_user_server.Models.Users.Selection
{
    public class Selected_ThemeTbl
    {
        public bool Deleted { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public byte Light { get; set; }
        public byte Night { get; set; }
        public byte Custom { get; set; }
    }
}
