using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mpc_dotnetc_user_server.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Birth_DateTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Month = table.Column<byte>(type: "INTEGER", nullable: false),
                    Day = table.Column<byte>(type: "INTEGER", nullable: false),
                    Year = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Birth_DateTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Completed_Email_RegistrationTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Completed_Email_RegistrationTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Discord_Email_AddressTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discord_Email_AddressTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Discord_IDsTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Discord_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Discord_User_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Discord_IDsTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Friends_PermissionTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Participant_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Approved = table.Column<bool>(type: "INTEGER", nullable: false),
                    Requested = table.Column<bool>(type: "INTEGER", nullable: false),
                    Blocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends_PermissionTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "IdentityTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Gender = table.Column<byte>(type: "INTEGER", nullable: false),
                    First_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Middle_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Last_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Maiden_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Ethnicity = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Logout_Time_Stamp_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Logout_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logout_Time_Stamp_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Pending_Email_Registration_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pending_Email_Registration_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Pending_Email_RegistrationTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pending_Email_RegistrationTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Profile_PageTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Page_Title = table.Column<string>(type: "TEXT", nullable: true),
                    Page_Description = table.Column<string>(type: "TEXT", nullable: true),
                    About_Me = table.Column<string>(type: "TEXT", nullable: true),
                    Banner_URL = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Profile_PageTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Email_RegistrationTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Email_RegistrationTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Client_ID_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Controller = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Client_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Server_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Client_ID_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_JWT_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Login_type = table.Column<string>(type: "TEXT", nullable: false),
                    JWT_issuer_key = table.Column<string>(type: "TEXT", nullable: false),
                    JWT_client_key = table.Column<string>(type: "TEXT", nullable: false),
                    JWT_client_address = table.Column<string>(type: "TEXT", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Controller = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Client_id = table.Column<long>(type: "INTEGER", nullable: false),
                    JWT_id = table.Column<long>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_JWT_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Logout_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Controller = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Client_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Server_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Logout_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Pending_Email_Registration_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Controller = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Pending_Email_Registration_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Unregistered_Email_Login_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Unregistered_Email_Login_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_User_Agent_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Client_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Server_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Controller = table.Column<string>(type: "TEXT", nullable: false),
                    Login_type = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_User_Agent_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reported_Broken_LinkTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    URL = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reported_Broken_LinkTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reported_Discord_Bot_BugTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Detail = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reported_Discord_Bot_BugTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reported_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Participant_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Block = table.Column<long>(type: "INTEGER", nullable: false),
                    Spam = table.Column<long>(type: "INTEGER", nullable: false),
                    Abuse = table.Column<long>(type: "INTEGER", nullable: false),
                    Fake = table.Column<long>(type: "INTEGER", nullable: false),
                    Nudity = table.Column<long>(type: "INTEGER", nullable: false),
                    Violence = table.Column<long>(type: "INTEGER", nullable: false),
                    Threat = table.Column<long>(type: "INTEGER", nullable: false),
                    Misinform = table.Column<long>(type: "INTEGER", nullable: false),
                    Harass = table.Column<long>(type: "INTEGER", nullable: false),
                    Illegal = table.Column<long>(type: "INTEGER", nullable: false),
                    Self_harm = table.Column<long>(type: "INTEGER", nullable: false),
                    Disruption = table.Column<long>(type: "INTEGER", nullable: false),
                    Hate = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reported_HistoryTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reported_ProfileTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Reported_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Avatar_Title = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar_URL = table.Column<string>(type: "TEXT", nullable: true),
                    Page_Title = table.Column<string>(type: "TEXT", nullable: true),
                    Page_Description = table.Column<string>(type: "TEXT", nullable: true),
                    About_Me = table.Column<string>(type: "TEXT", nullable: true),
                    Banner_URL = table.Column<string>(type: "TEXT", nullable: true),
                    Reported_Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Report_Chat_TS = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reported_ProfileTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reported_ReasonTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Reported_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reported_ReasonTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Reported_Website_BugTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    URL = table.Column<string>(type: "TEXT", nullable: false),
                    Detail = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reported_Website_BugTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "ReportedTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Block = table.Column<long>(type: "INTEGER", nullable: false),
                    Spam = table.Column<long>(type: "INTEGER", nullable: false),
                    Abuse = table.Column<long>(type: "INTEGER", nullable: false),
                    Fake = table.Column<long>(type: "INTEGER", nullable: false),
                    Nudity = table.Column<long>(type: "INTEGER", nullable: false),
                    Violence = table.Column<long>(type: "INTEGER", nullable: false),
                    Threat = table.Column<long>(type: "INTEGER", nullable: false),
                    Misinform = table.Column<long>(type: "INTEGER", nullable: false),
                    Harass = table.Column<long>(type: "INTEGER", nullable: false),
                    Illegal = table.Column<long>(type: "INTEGER", nullable: false),
                    Self_harm = table.Column<long>(type: "INTEGER", nullable: false),
                    Disruption = table.Column<long>(type: "INTEGER", nullable: false),
                    Hate = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportedTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_App_AlignmentTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Left = table.Column<bool>(type: "INTEGER", nullable: false),
                    Right = table.Column<bool>(type: "INTEGER", nullable: false),
                    Center = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_App_AlignmentTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_App_Grid_TypeTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Grid = table.Column<byte>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_App_Grid_TypeTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_App_Text_AlignmentTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Left = table.Column<bool>(type: "INTEGER", nullable: false),
                    Right = table.Column<bool>(type: "INTEGER", nullable: false),
                    Center = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_App_Text_AlignmentTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_Avatar_TitleTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Avatar_title = table.Column<string>(type: "TEXT", nullable: false),
                    Avatar_url_path = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_Avatar_TitleTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_AvatarTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: true),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: true),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: true),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: true),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: true),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: true),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: true),
                    Avatar_title = table.Column<string>(type: "TEXT", nullable: true),
                    Avatar_url_path = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_AvatarTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_LanguageTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Language_code = table.Column<string>(type: "TEXT", nullable: false),
                    Region_code = table.Column<string>(type: "TEXT", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_LanguageTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_NameTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: true),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: true),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: true),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: true),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_NameTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_Navbar_LockTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Locked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_Navbar_LockTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_StatusTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Online = table.Column<bool>(type: "INTEGER", nullable: false),
                    Offline = table.Column<bool>(type: "INTEGER", nullable: false),
                    Hidden = table.Column<bool>(type: "INTEGER", nullable: false),
                    Away = table.Column<bool>(type: "INTEGER", nullable: false),
                    DND = table.Column<bool>(type: "INTEGER", nullable: false),
                    Custom = table.Column<bool>(type: "INTEGER", nullable: false),
                    Custom_lbl = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_StatusTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Selected_ThemeTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Light = table.Column<bool>(type: "INTEGER", nullable: false),
                    Night = table.Column<bool>(type: "INTEGER", nullable: false),
                    Custom = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_ThemeTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Twitch_Email_AddressTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Twitch_Email_AddressTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Twitch_IDsTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Twitch_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    User_Name = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Twitch_IDsTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "User_IDsTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Public_ID = table.Column<string>(type: "TEXT", nullable: false),
                    Secret_ID = table.Column<string>(type: "TEXT", nullable: false),
                    Secret_Hash_ID = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User_IDsTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "WebSocket_Chat_PermissionTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Participant_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Approved = table.Column<bool>(type: "INTEGER", nullable: false),
                    Requested = table.Column<bool>(type: "INTEGER", nullable: false),
                    Blocked = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WebSocket_Chat_PermissionTbl", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Account_GroupsTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Groups = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account_GroupsTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Account_GroupsTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Account_RolesTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Roles = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account_RolesTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Account_RolesTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Account_TypeTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Account_TypeTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Account_TypeTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comment_BoxTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comment_BoxTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Comment_BoxTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Contact_UsTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Subject_Line = table.Column<string>(type: "TEXT", nullable: false),
                    Summary = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contact_UsTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Contact_UsTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Login_Email_AddressTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login_Email_AddressTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Login_Email_AddressTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Login_PasswordTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Password = table.Column<byte[]>(type: "BLOB", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login_PasswordTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Login_PasswordTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Login_Time_Stamp_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Login_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login_Time_Stamp_HistoryTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Login_Time_Stamp_HistoryTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Login_Time_StampTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Login_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Login_Time_StampTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Login_Time_StampTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logout_Time_StampTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Logout_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logout_Time_StampTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Logout_Time_StampTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Email_Login_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Email_Login_HistoryTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Report_Failed_Email_Login_HistoryTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Email_Logout_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Email_Logout_HistoryTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Report_Failed_Email_Logout_HistoryTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Report_Failed_Selected_HistoryTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_time = table.Column<long>(type: "INTEGER", nullable: false),
                    Client_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Server_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Remote_IP = table.Column<string>(type: "TEXT", nullable: false),
                    Client_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Server_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Remote_Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Language_Region = table.Column<string>(type: "TEXT", nullable: false),
                    Email_Address = table.Column<string>(type: "TEXT", nullable: false),
                    Location = table.Column<string>(type: "TEXT", nullable: false),
                    Reason = table.Column<string>(type: "TEXT", nullable: false),
                    Action = table.Column<string>(type: "TEXT", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false),
                    Controller = table.Column<string>(type: "TEXT", nullable: false),
                    User_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Client_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Server_user_agent = table.Column<string>(type: "TEXT", nullable: false),
                    Down_link = table.Column<string>(type: "TEXT", nullable: false),
                    Connection_type = table.Column<string>(type: "TEXT", nullable: false),
                    RTT = table.Column<string>(type: "TEXT", nullable: false),
                    Data_saver = table.Column<string>(type: "TEXT", nullable: false),
                    Device_ram_gb = table.Column<string>(type: "TEXT", nullable: false),
                    Orientation = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_width = table.Column<string>(type: "TEXT", nullable: false),
                    Screen_height = table.Column<string>(type: "TEXT", nullable: false),
                    Color_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Pixel_depth = table.Column<string>(type: "TEXT", nullable: false),
                    Window_width = table.Column<string>(type: "TEXT", nullable: false),
                    Window_height = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Report_Failed_Selected_HistoryTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Report_Failed_Selected_HistoryTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Selected_App_Custom_DesignTbl",
                columns: table => new
                {
                    ID = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    End_User_ID = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Created_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Updated_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_on = table.Column<long>(type: "INTEGER", nullable: false),
                    Deleted_by = table.Column<long>(type: "INTEGER", nullable: false),
                    Card_Border_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Header_Font = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Header_Background_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Header_Font_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Body_Font = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Body_Background_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Body_Font_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Footer_Font = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Footer_Background_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Card_Footer_Font_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Navigation_Menu_Background_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Navigation_Menu_Font_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Navigation_Menu_Font = table.Column<string>(type: "TEXT", nullable: false),
                    Button_Background_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Button_Font_Color = table.Column<string>(type: "TEXT", nullable: false),
                    Button_Font = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Selected_App_Custom_DesignTbl", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Selected_App_Custom_DesignTbl_User_IDsTbl_End_User_ID",
                        column: x => x.End_User_ID,
                        principalTable: "User_IDsTbl",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Account_GroupsTbl_End_User_ID",
                table: "Account_GroupsTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Account_RolesTbl_End_User_ID",
                table: "Account_RolesTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Account_TypeTbl_End_User_ID",
                table: "Account_TypeTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Comment_BoxTbl_End_User_ID",
                table: "Comment_BoxTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UsTbl_End_User_ID",
                table: "Contact_UsTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Login_Email_AddressTbl_End_User_ID",
                table: "Login_Email_AddressTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Login_PasswordTbl_End_User_ID",
                table: "Login_PasswordTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Login_Time_Stamp_HistoryTbl_End_User_ID",
                table: "Login_Time_Stamp_HistoryTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Login_Time_StampTbl_End_User_ID",
                table: "Login_Time_StampTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Logout_Time_StampTbl_End_User_ID",
                table: "Logout_Time_StampTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Failed_Email_Login_HistoryTbl_End_User_ID",
                table: "Report_Failed_Email_Login_HistoryTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Failed_Email_Logout_HistoryTbl_End_User_ID",
                table: "Report_Failed_Email_Logout_HistoryTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Report_Failed_Selected_HistoryTbl_End_User_ID",
                table: "Report_Failed_Selected_HistoryTbl",
                column: "End_User_ID");

            migrationBuilder.CreateIndex(
                name: "IX_Selected_App_Custom_DesignTbl_End_User_ID",
                table: "Selected_App_Custom_DesignTbl",
                column: "End_User_ID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Account_GroupsTbl");

            migrationBuilder.DropTable(
                name: "Account_RolesTbl");

            migrationBuilder.DropTable(
                name: "Account_TypeTbl");

            migrationBuilder.DropTable(
                name: "Birth_DateTbl");

            migrationBuilder.DropTable(
                name: "Comment_BoxTbl");

            migrationBuilder.DropTable(
                name: "Completed_Email_RegistrationTbl");

            migrationBuilder.DropTable(
                name: "Contact_UsTbl");

            migrationBuilder.DropTable(
                name: "Discord_Email_AddressTbl");

            migrationBuilder.DropTable(
                name: "Discord_IDsTbl");

            migrationBuilder.DropTable(
                name: "Friends_PermissionTbl");

            migrationBuilder.DropTable(
                name: "IdentityTbl");

            migrationBuilder.DropTable(
                name: "Login_Email_AddressTbl");

            migrationBuilder.DropTable(
                name: "Login_PasswordTbl");

            migrationBuilder.DropTable(
                name: "Login_Time_Stamp_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Login_Time_StampTbl");

            migrationBuilder.DropTable(
                name: "Logout_Time_Stamp_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Logout_Time_StampTbl");

            migrationBuilder.DropTable(
                name: "Pending_Email_Registration_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Pending_Email_RegistrationTbl");

            migrationBuilder.DropTable(
                name: "Profile_PageTbl");

            migrationBuilder.DropTable(
                name: "Report_Email_RegistrationTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Client_ID_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Email_Login_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Email_Logout_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_JWT_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Logout_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Pending_Email_Registration_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Selected_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_Unregistered_Email_Login_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Report_Failed_User_Agent_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Reported_Broken_LinkTbl");

            migrationBuilder.DropTable(
                name: "Reported_Discord_Bot_BugTbl");

            migrationBuilder.DropTable(
                name: "Reported_HistoryTbl");

            migrationBuilder.DropTable(
                name: "Reported_ProfileTbl");

            migrationBuilder.DropTable(
                name: "Reported_ReasonTbl");

            migrationBuilder.DropTable(
                name: "Reported_Website_BugTbl");

            migrationBuilder.DropTable(
                name: "ReportedTbl");

            migrationBuilder.DropTable(
                name: "Selected_App_AlignmentTbl");

            migrationBuilder.DropTable(
                name: "Selected_App_Custom_DesignTbl");

            migrationBuilder.DropTable(
                name: "Selected_App_Grid_TypeTbl");

            migrationBuilder.DropTable(
                name: "Selected_App_Text_AlignmentTbl");

            migrationBuilder.DropTable(
                name: "Selected_Avatar_TitleTbl");

            migrationBuilder.DropTable(
                name: "Selected_AvatarTbl");

            migrationBuilder.DropTable(
                name: "Selected_LanguageTbl");

            migrationBuilder.DropTable(
                name: "Selected_NameTbl");

            migrationBuilder.DropTable(
                name: "Selected_Navbar_LockTbl");

            migrationBuilder.DropTable(
                name: "Selected_StatusTbl");

            migrationBuilder.DropTable(
                name: "Selected_ThemeTbl");

            migrationBuilder.DropTable(
                name: "Twitch_Email_AddressTbl");

            migrationBuilder.DropTable(
                name: "Twitch_IDsTbl");

            migrationBuilder.DropTable(
                name: "WebSocket_Chat_PermissionTbl");

            migrationBuilder.DropTable(
                name: "User_IDsTbl");
        }
    }
}
