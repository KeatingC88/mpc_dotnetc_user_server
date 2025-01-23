﻿using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation
{
    public class Submit_Email_RegistrationDTO
    {
        [Required(ErrorMessage = "Email Address is Missing.")]
        public string Email_Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is Missing.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Language_Code is Missing.")]
        public string Language { get; set; } = string.Empty;

        [Required(ErrorMessage = "Region_Code is Missing.")]
        public string Region { get; set; } = string.Empty;

        [Required(ErrorMessage = "Code is Missing.")]
        public string Code { get; set; } = string.Empty;

        [Required(ErrorMessage = "Location is Missing.")]
        public string Location { get; set; } = string.Empty;

        [Required(ErrorMessage = "Client Time is Missing.")]
        public string Client_time { get; set; } = string.Empty;

        [Required(ErrorMessage = "Theme is Missing.")]
        public string Theme { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alignment is Missing.")]
        public string Alignment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alignment is Missing.")]
        public string Text_alignment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nav_Lock is Missing.")]
        public string Nav_lock { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grid Type is Missing.")]
        public string Grid_type { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Issuer Key is Missing.")]
        public string JWT_issuer_key { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Client Key is Missing.")]
        public string JWT_client_key { get; set; } = string.Empty;

        [Required(ErrorMessage = "JWT Client Address is Missing.")]
        public string JWT_client_address { get; set; } = string.Empty;
    }
}
