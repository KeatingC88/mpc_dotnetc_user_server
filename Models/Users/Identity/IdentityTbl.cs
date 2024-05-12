namespace dotnet_user_server.Models.Users.Identity
{
    public class IdentityTbl
    {
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Created_on { get; set; }
        public byte Deleted { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public byte B_Month { get; set; }
        public byte B_Day { get; set; }
        public ulong B_Year { get; set; }
        public byte Gender { get; set; }
        public string? First_Name { get; set; } = string.Empty;
        public string? Middle_Name { get; set; } = string.Empty;
        public string? Last_Name { get; set; } = string.Empty;
        public string? Maiden_Name { get; set; } = string.Empty;
        public string? Ethnicity { get; set; } = string.Empty;

    }
}