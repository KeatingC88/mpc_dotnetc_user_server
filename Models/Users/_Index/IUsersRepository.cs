namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public interface IUsersRepository
    {
        Task<bool> Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(string email_address);
        Task<bool> Email_Exists_In_Login_Email_Address_Tbl(string email_address);
        Task<bool> Update_User_Password(DTO dto);
        Task<bool> Update_Chat_Web_Socket_Permissions_Tbl(DTO dto);
        Task<bool> Create_Reported_WebSocket_Record(DTO dto);
        Task<bool> Create_Comment_Box_Record(DTO dto);
        Task<bool> Create_Broken_Link_Record(DTO dto);
        Task<bool> Create_Discord_Bot_Bug_Record(DTO dto);
        Task<bool> Create_WebSocket_Log_Record(DTO dto);
        Task<bool> Create_Website_Bug_Record(DTO dto);
        Task<bool> Create_Contact_Us_Record(DTO dto);
        Task<byte[]> Create_Salted_Hash_String(byte[] text, byte[] salt);
        bool Compare_Password_Byte_Arrays(byte[] array1, byte[] array2);
        Task<bool> Confirmation_Code_Exists_In_Not_Confirmed_Email_Address_Tbl(string Code);
        Task<bool> Telephone_Exists_In_Login_Telephone_Tbl(string telephone_number);
        Task<bool> Phone_Exists_In_Telephone_Not_Confirmed_Tbl(string telephone_number);
        Task<bool> ID_Exists_In_Users_Tbl(ulong id);
        Task<byte> Read_End_User_Selected_Status(DTO dto);
        Task<byte[]?> Get_User_Password_Hash_By_ID(ulong id);
        Task<ulong> Get_User_ID_By_Email_Address(string email_address);
        Task<ulong> Get_User_ID_From_JWToken(string token);
        Task<string> Create_Unconfirmed_Email(DTO dto);
        Task<string> Create_Unconfirmed_Phone(DTO dto);
        Task<string> Create_Account_By_Email(DTO dto);
        Task<string> Create_Account_By_Phone(DTO dto);
        Task<string> Create_Reported_User_Profile_Record(DTO dto);
        Task<string> Delete_Account_By_User_ID(DTO dto);
        Task<string> Read_User(DTO dto);
        Task<string> Read_User_Profile(DTO dto);
        Task<string> Read_WebSocket_Permission_Record(DTO dto);
        Task<string> Read_End_User_Web_Socket_Data(DTO dto);
        Task<string> Update_Unconfirmed_Phone(DTO dto);
        Task<string> Update_Unconfirmed_Email(DTO dto);
        Task<string> Update_User_Avatar(DTO dto);
        Task<string> Update_User_Display_Name(DTO dto);
        Task<string> Update_User_Login(DTO dto);
        Task<string> Update_User_Logout(ulong user_id);
        Task<string> Update_User_Selected_Alignment(DTO dto);
        Task<string> Update_User_Selected_TextAlignment(DTO dto);
        Task<string> Update_User_Selected_Language(DTO dto);
        Task<string> Update_User_Selected_Nav_Lock(DTO dto);
        Task<string?> Get_User_Email_By_ID(ulong id);
        Task<string> Update_User_Selected_Status(DTO dto);
        Task<string> Update_User_Selected_Theme(DTO dto);
        Task<string> Create_Jwt_Token(string id);
        Task<string> Create_Integration_Twitch_Record(DTO dto);
        Task<string> Save_End_User_First_Name(DTO dto);
        Task<string> Save_End_User_Last_Name(DTO dto);
        Task<string> Save_End_User_Middle_Name(DTO dto);
        Task<string> Save_End_User_Maiden_Name(DTO dto);
        Task<string> Save_End_User_Ethnicity(DTO dto);
        Task<string> Save_End_User_Birth_Date(DTO dto);
        Task<string> Update_End_User_Gender(DTO dto);
        Task<string> Read_Users();
        void Create_Chat_WebSocket_Log_Records(DTO dto);
        void Create_End_User_Database_Fields(dynamic dto);
    }
    public class DTO
    {
        public ulong ID { get; set; }
        public ulong User_ID { get; set; }
        public ulong Year { get; set; }
        public ulong Target_ID { get; set; }
        public ulong Sent_to { get; set; }
        public ulong Sent_from { get; set; }
        public ulong Updated_by { get; set; }
        public ulong Deleted_on { get; set; }
        public ulong Deleted_by { get; set; }
        public ulong Timestamp { get; set; }
        public ulong Created_on { get; set; }
        public ulong Updated_on { get; set; }
        public ulong Login_on { get; set; }
        public ulong Logout_on { get; set; }
        public ulong Send_to { get; set; }
        public ulong Reported_ID { get; set; }
        public ulong? Report_chat_TS { get; set; }

        public DateTime Token_expire { get; set; }

        public string Avatar_title { get; set; } = string.Empty;
        public string Avatar_url_path { get; set; } = string.Empty;
        public string Carrier { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public string Custom_lbl { get; set; } = string.Empty;
        public string Detail { get; set; } = string.Empty;
        public string Display_name { get; set; } = string.Empty;
        public string Email_Address { get; set; } = string.Empty;
        public string Ethnicity { get; set; } = string.Empty;
        public string First_name { get; set; } = string.Empty;
        public string Language { get; set; } = string.Empty;
        public string Language_code { get; set; } = string.Empty;
        public string Last_name { get; set; } = string.Empty;
        public string Maiden_name { get; set; } = string.Empty;
        public string? Message { get; set; } = string.Empty;
        public string Middle_name { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string New_password { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public string Region_code { get; set; } = string.Empty;
        public string Reported_Reason { get; set; } = string.Empty;

        public string Subject_line { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string Target_name { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string URL { get; set; } = string.Empty;

        public bool Deleted { get; set; }
        public bool Nav_lock { get; set; }

        public byte Approved { get; set; }
        public byte Blocked { get; set; }
        public byte Country { get; set; }
        public byte Gender { get; set; }
        public byte Month { get; set; }
        public byte Day { get; set; }
        public byte Online_status { get; set; }
        public byte Requested { get; set; }
        public byte Theme { get; set; }
        public byte Spam { get; set; } 
        public byte Abuse { get; set; } 
        public byte Alignment { get; set; }
        public byte Text_alignment { get; set; }
    }
}
