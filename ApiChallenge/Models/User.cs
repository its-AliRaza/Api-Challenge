using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiChallenge.Models
{
    public class User
    {
        public int U_ID { get; set; }

        [Required(ErrorMessage = "This feild is mandatory.")]
        [Display(Name = "Full Name")]
        public string U_Name { get; set; }

        [Required(ErrorMessage = "This feild is mandatory.")]
        [Display(Name = "Email")]
        public string U_Email { get; set; }

        [Required(ErrorMessage = "This feild is mandatory.")]
        [Display(Name = "Password")]
        public string U_Password { get; set; }
        public string Token { get; set; }
    }
}