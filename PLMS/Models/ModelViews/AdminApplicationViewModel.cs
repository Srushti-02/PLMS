using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLMS.Models.ModelViews
{
    public class AdminApplicationViewModel
    {
        public int ApplicationId { get; set; }
        public string ApplicantName { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string PANNumber { get; set; }
        public string AadhaarNumber { get; set; }
        public string CompanyName { get; set; }
        public decimal MonthlyIncome { get; set; }

        public int? AssignedOfficerId { get; set; }
        public string AssignedOfficerName { get; set; }

        public IEnumerable<SelectListItem> OfficersList { get; set; }
    }

}