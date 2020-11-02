using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ApiChallenge.Models
{
    public class Developer
    {
        public int D_ID { get; set; }

        public string U_ID { get; set; }

        [Required(ErrorMessage = "Developer Name is required.")]
        [Display(Name = "Developer Name")]
        public string D_Name { get; set; }

        [Required(ErrorMessage = "Developer Email is required.")]
        [Display(Name = "Developer Email")]
        public string D_Email { get; set; }

        [Required(ErrorMessage = "User Email is required.")]
        [Display(Name = "User Email")]
        public string U_Email { get; set; }

        [Required(ErrorMessage = "Age is required.")]
        [Display(Name = "Age")]
        public string D_Age { get; set; }

        public string D_CreatedAt { get; set; }

        public string D_CreatedBy { get; set; }
    }

    public class TotalDeveloper
    {
        public List<Developer> Reports { get; set; } = new List<Developer>();
        public int TotalPages { get; set; }
    }

    public class DeveloperRequest
    {
        [Required(ErrorMessage = "From  Date is required.")]
        [Display(Name = "From Date")]
        public string From { get; set; }
        [Required(ErrorMessage = "To  Date is required.")]
        [Display(Name = "To Date")]
        public string To { get; set; }
        public int Page { get; set; } = 1;
    }
}