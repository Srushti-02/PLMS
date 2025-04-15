using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLMS.Models.DbModel;
using PLMS.Models.ModelViews;

namespace PLMS.Controllers
{
    public class OfficerController : Controller
    {
        G1IBMDbEntities _db = new G1IBMDbEntities();

        public ActionResult Dashboard()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = Convert.ToInt32(Session["userID"]);

            // Find officer using userID (foreign key)
            var officer = _db.Officers.FirstOrDefault(o => o.userID == userId);

            if (officer == null)
            {
                TempData["Error"] = "Officer profile not found for this user.";
                return RedirectToAction("Index", "Home");
            }

            int officerId = officer.officerID;

            var assignedApplications = _db.Officers
                .Where(o => o.officerID == officerId)
                .Select(o => new LOViewModel
                {
                    ApplicationId = o.applicationID,
                    ApplicantName = o.LoanApplication.Applicant.username,
                    Email = o.LoanApplication.email,
                    Address = o.LoanApplication.address,
                    CompanyName = o.LoanApplication.companyName,
                    Status = o.LoanApplication.LoanStatu.loanStatus,
                    Remark = o.LoanApplication.LoanStatu.remark
                }).ToList();

            ViewBag.Username = _db.USERs.FirstOrDefault(u => u.userID == userId)?.fullName;

            return View(assignedApplications);
        }


        [HttpPost]
        public ActionResult UpdateStatus(int applicationId, string decision)
        {
            var loanStatus = _db.LoanStatus.FirstOrDefault(s => s.applicationID == applicationId);

            if (loanStatus != null)
            {
                loanStatus.loanStatus = decision == "Approve" ? "Approved" : "Rejected";
                loanStatus.remark = decision == "Approve" ? "Approved by Loan Officer" : "Rejected by Loan Officer";

                _db.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }
    }

}