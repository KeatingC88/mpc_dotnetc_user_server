using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Report
{
    public class Reported_Reason
    {
        public long Created_by { get; set; }
        public long Created_on { get; set; }
        public bool Deleted { get; set; }
        public long Deleted_on { get; set; }
        public long Deleted_by { get; set; }
        public long Updated_on { get; set; }
        public long Updated_by { get; set; }
        public long Reported_ID { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
