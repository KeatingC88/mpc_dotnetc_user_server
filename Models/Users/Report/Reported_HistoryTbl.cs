using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Report
{
    public class Reported_HistoryTbl
    {
        public ulong Created_by { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong User_ID { get; set; }
        public ulong Participant_ID { get; set; }
        public ulong Block { get; set; }
        public ulong Spam { get; set; }
        public ulong Abuse { get; set; }
        public ulong Fake { get; set; }
        public ulong Nudity { get; set; }
        public ulong Violence { get; set; }
        public ulong Threat { get; set; }
        public ulong Misinform { get; set; }
        public ulong Harass { get; set; }
        public ulong Illegal { get; set; }
        public ulong Self_harm { get; set; }
        public ulong Disruption { get; set; }
        public ulong Hate { get; set; }
        public ulong ID { get; set; }
    }
}
