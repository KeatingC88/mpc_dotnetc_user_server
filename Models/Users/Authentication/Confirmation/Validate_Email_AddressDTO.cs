using System.ComponentModel.DataAnnotations;
using mpc_dotnetc_user_server.Models.Users.Selection;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Validate_Email_AddressDTO
    {
        
        public string Email_Address { get; set; } = string.Empty;
        
        public string Language { get; set; } = string.Empty;
        
        public string Region { get; set; } = string.Empty;
        
        public string Location { get;set; } = string.Empty;
        
        public string Client_time { get; set; } = string.Empty;
        
        public string JWT_issuer_key { get; set; } = string.Empty;

        
        public string JWT_client_key { get; set; } = string.Empty;

        
        public string JWT_client_address { get; set; } = string.Empty;

        
        public string User_agent { get; set; } = string.Empty;

        public string Client_user_agent { get; set; } = string.Empty;

        public string Server_user_agent { get; set; } = string.Empty;
        

        public string Down_link { get; set; } = string.Empty;
        

        public string Connection_type { get; set; } = string.Empty;
        

        public string RTT { get; set; } = string.Empty;
        

        public string Data_saver { get; set; } = string.Empty;
        

        public string Device_ram_gb { get; set; } = string.Empty;
        

        public string Orientation { get; set; } = string.Empty;
        

        public string Screen_width { get; set; } = string.Empty;
        

        public string Screen_height { get; set; } = string.Empty;
        

        public string Screen_extend { get; set; } = string.Empty;
        

        public string Color_depth { get; set; } = string.Empty;
        

        public string Pixel_depth { get; set; } = string.Empty;
        

        public string Window_width { get; set; } = string.Empty;
        

        public string Window_height { get; set; } = string.Empty;

    }
}