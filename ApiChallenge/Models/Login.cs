using System.ComponentModel.DataAnnotations;

namespace ApiChallenge.Models
{
    public class Login
    {
        [Required(ErrorMessage = "This feild is mandatory.")]
        [Display(Name = "Email")]
        public string U_Email { get; set; }

        [Required(ErrorMessage = "This feild is mandatory.")]
        [Display(Name = "Password")]
        public string U_Password { get; set; }
    }
}