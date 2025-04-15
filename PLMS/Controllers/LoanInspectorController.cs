using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLMS.Models.DbModel;
using PLMS.Models.ModelViews;

namespace PLMS.Controllers
{
    public class LoanInspectorController : Controller
    {
        G1IBMDbEntities _db = new G1IBMDbEntities();
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["username"] == null)
            {
                // Redirect to login page if session is not set
                filterContext.Result = RedirectToAction("Login", "Home");
            }

            base.OnActionExecuting(filterContext);
        }
        public ActionResult Dashboard()
        {
            if (Session["userID"] == null)
                return RedirectToAction("Login", "Home");

            int userId = Convert.ToInt32(Session["userID"]);

            var assignedApplications = _db.Officers
                .Where(o => o.userID == userId && o.LoanApplication.LoanStatu.remark == "Approved by Loan Officer")
                .Select(o => new LoanInspectorViewModel
                {
                    ApplicationId = o.applicationID,
                    ApplicantName = o.LoanApplication.Applicant.username,
                    Email = o.LoanApplication.email,
                    Address = o.LoanApplication.address,
                    CompanyName = o.LoanApplication.companyName,
                    Status = o.LoanApplication.LoanStatu.loanStatus,
                    Remark = o.LoanApplication.LoanStatu.remark,
                    RegistrationID = o.LoanApplication.registrationID,
                    applicantId = o.LoanApplication.applicationID,
                }).ToList();

            ViewBag.Username = _db.USERs.FirstOrDefault(u => u.userID == userId)?.fullName;
            return View(assignedApplications);
        }


        public JsonResult ViewHistory(int applicantId)
        {
            var registrationID = _db.LoanApplications
                .Where(a => a.applicationID == applicantId)
                .Select(a => a.registrationID)
                .FirstOrDefault();

            var pastApplications = _db.LoanApplications
                .Where(l => l.registrationID == registrationID)
                .Select(l => new
                {
                    l.applicationID,
                    l.email,
                    l.companyName,
                    loanStatus = l.LoanStatu.loanStatus,
                    l.LoanStatu.remark
                }).ToList();

            return Json(pastApplications, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateStatus(int applicationId, string status)
        {
            var loanStatus = _db.LoanStatus.FirstOrDefault(ls => ls.applicationID == applicationId);
            if (loanStatus != null)
            {
                loanStatus.loanStatus = status;
                loanStatus.remark = status == "Approved" ? "Approved by Loan Inspector" : "Rejected by Loan Inspector";
                _db.SaveChanges();
                return new HttpStatusCodeResult(200);
            }
            return new HttpStatusCodeResult(404, "Loan application not found");
        }




    }
}