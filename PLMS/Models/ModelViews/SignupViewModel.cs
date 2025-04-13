using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PLMS.Models.ModelViews
{
	public class SignupViewModel
	{
        [Required]
        [Display(Name = "Full Name")]
        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "Use letters only please")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Contact Number")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Enter a valid 10-digit number")]
        public string ContactNumber { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [RegularExpression(@"^[A-Za-z0-9@#$!]*$", ErrorMessage = "Use valid characters only")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }
    }
}