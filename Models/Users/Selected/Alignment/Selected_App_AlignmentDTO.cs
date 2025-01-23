﻿using System.ComponentModel.DataAnnotations;

namespace mpc_dotnetc_user_server.Models.Users.Selected.Alignment
{
    public class Selected_App_AlignmentDTO
    {
        [Required]
        public string Alignment { get; set; } = string.Empty;

        [Required(ErrorMessage = "Application Token is Missing.")]
        public string Token { get; set; } = string.Empty;

        public ulong User_id { get; set; }
    }
}
