namespace dotnet_user_server.Models.Users.Selections
{
    public class Selected_NameTbl
    {
        public bool Deleted { get; set; }
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
