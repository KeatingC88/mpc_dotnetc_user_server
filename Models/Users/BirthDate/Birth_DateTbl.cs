﻿namespace mpc_dotnetc_user_server.Models.Users.BirthDate
{
    public class Birth_DateTbl
    {
        public ulong ID { get; set; }
        public ulong User_id { get; set; }
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public byte Month { get; set; }
        public byte Day { get; set; }
        public ulong Year { get; set; }
    }
}