using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PLMS.Models.ModelViews
{
	public class ApplicantViewModel
	{
        public int ApplicationId { get; set; }
        public DateTime SubmissionDate { get; set; }
        public string Status { get; set; }

        public string RegistrationId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string AadhaarNumber { get; set; }
        public string PANNumber { get; set; }
        public DateTime DOB { get; set; }
        public decimal MonthlyIncome { get; set; }
        public string CompanyName { get; set; }

        // For image upload
        public HttpPostedFileBase Photo { get; set; }
    }
}