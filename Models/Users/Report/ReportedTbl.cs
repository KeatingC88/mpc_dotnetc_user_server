using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Report
{
    public class ReportedTbl
    {
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Updated_on { get; set; }
        public long Updated_by { get; set; }
        public long End_User_ID { get; set; }
        public long Block { get; set; }
        public long Spam { get; set; }
        public long Abuse { get; set; }
        public long Fake { get; set; }
        public long Nudity { get; set; }
        public long Violence { get; set; }
        public long Threat { get; set; }
        public long Misinform { get; set; }
        public long Harass { get; set; }
        public long Illegal { get; set; }
        public long Self_harm { get; set; }
        public long Disruption { get; set; }
        public long Hate { get; set; }
        public long ID { get; set; }
    }
}
