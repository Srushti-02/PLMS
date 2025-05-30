﻿using System;
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
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["username"] == null)
            {
                // Redirect to login page if session is not set
                filterContext.Result = RedirectToAction("Login", "Home");
            }
            string userRole = Session["role"] as string;
            if (Session["username"] != null && userRole != "LO")
            {
                filterContext.Result = RedirectToAction("Login", "Home");
            }

            var response = filterContext.HttpContext.Response;
            response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            response.Cache.SetValidUntilExpires(false);
            response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();
            base.OnActionExecuting(filterContext);
        }
        public ActionResult Dashboard()
        {
            if (Session["userID"] == null)
            {
                return RedirectToAction("Login", "Home");
            }

            int userId = Convert.ToInt32(Session["userID"]);

            // Get the current officer's ID
            var officer = _db.Officers.FirstOrDefault(o => o.userID == userId);
            if (officer == null)
            {
                TempData["Error"] = "Officer profile not found for this user.";
                return RedirectToAction("Index", "Home");
            }

            int officerId = officer.officerID;

            // Get all applications assigned to this officer with 'Pending' status
            var assignedApplications = _db.Officers
                .Where(o => o.userID == userId && o.LoanApplication.LoanStatu.loanStatus == "Pending")
                .Select(o => new LOViewModel
                {
                    ApplicationId = o.applicationID,
                    ApplicantName = o.LoanApplication.Applicant.fullName,
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
        public ActionResult UpdateStatus(int applicationId, string status)
        {
            var loanStatus = _db.LoanStatus.FirstOrDefault(s => s.applicationID == applicationId);

            if (loanStatus != null)
            {
                if (status == "Approved")
                {
                    loanStatus.loanStatus = "Approved";
                    loanStatus.remark = "Approved by Loan Officer";
                }
                else if (status == "Rejected")
                {
                    loanStatus.loanStatus = "Incomplete";
                    loanStatus.remark = "Incomplete Application Stated by Loan Officer";
                }


                    _db.SaveChanges();
            }

            return RedirectToAction("Dashboard");
        }

    }

}