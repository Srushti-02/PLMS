using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PLMS.Models.ModelViews
{
	public class ApplyEditViewModel
	{
        [Key]
        public int ApplicationId { get; set; }
        [Required]
        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "Use letters only please")]
        public string FullName { get; set; }

        public string Email { get; set; }
        [Required] 
        public string Address { get; set; }
        [Required]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Enter a valid 12-digit Adhar number")]
        public string AadhaarNumber { get; set; }
        [Required]
        [RegularExpression(@"^[A-Z]{5}[0-9]{4}[A-Z]{1}$", ErrorMessage = "Enter a valid 10-character PAN number (e.g., ABCDE1234F)")]
        public string PANNumber { get; set; }
        [Required]
        public DateTime DOB { get; set; }
        [Required]
        public decimal MonthlyIncome { get; set; }
        [Required]
        public string CompanyName { get; set; }
        public string Remark { get; set; }
    }
}