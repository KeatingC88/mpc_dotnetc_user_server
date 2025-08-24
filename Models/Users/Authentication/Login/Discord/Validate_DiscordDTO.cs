using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord
{
    public class Validate_DiscordDTO
    {
        [Required(ErrorMessage = "Theme is Missing.")]
        public string Theme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alignment is Missing.")]
        public string Alignment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Text_alignment is Missing.")]
        public string Text_alignment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nav_Lock is Missing.")]
        public string Nav_lock { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grid Type is Missing.")]
        public string Grid_type { get; set; } = string.Empty;

        [Required]
        public string Language { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public string Login_type { get; set; } = string.Empty;

        [Required]
        public string Client_time { get; set; } = string.Empty;

        public ulong Client_Time_Parsed { get; set; }

        [Required]
        public string Code { get; set; } = string.Empty;


        [Required]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required]
        public string JWT_client_address { get; set; } = string.Empty;

        [Required]
        public string User_agent { get; set; } = string.Empty;

        public string Client_user_agent { get; set; } = string.Empty;

        public string Server_user_agent { get; set; } = string.Empty;

        [Required]
        public string Down_link { get; set; } = string.Empty;

        [Required]
        public string Connection_type { get; set; } = string.Empty;

        [Required]
        public string RTT { get; set; } = string.Empty;

        [Required]
        public string Data_saver { get; set; } = string.Empty;

        [Required]
        public string Device_ram_gb { get; set; } = string.Empty;

        [Required]
        public string Orientation { get; set; } = string.Empty;

        [Required]
        public string Screen_width { get; set; } = string.Empty;

        [Required]
        public string Screen_height { get; set; } = string.Empty;

        [Required]
        public string Color_depth { get; set; } = string.Empty;

        [Required]
        public string Pixel_depth { get; set; } = string.Empty;

        [Required]
        public string Window_width { get; set; } = string.Empty;

        [Required]
        public string Window_height { get; set; } = string.Empty;
    }
}