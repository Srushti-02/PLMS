using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLMS.Models.DbModel;
using PLMS.Models.ModelViews;

namespace PLMS.Controllers
{
    
    public class ApplicantController : Controller
    {
        G1IBMDbEntities _db = new G1IBMDbEntities();
        // GET: Applicant
        public ActionResult ApplicantDashboard()
        {
            string username = Session["username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Home");
            }

            var applicant = _db.Applicants
                .Include("LoanApplications.LoanStatu") // Include related LoanApplications and LoanStatus
                .FirstOrDefault(a => a.username == username);

            if (applicant == null)
            {
                TempData["FailureMessage"] = "Applicant not found.";
                return RedirectToAction("Login", "Home");
            }

            // Prepare ViewModel from the LoanApplications of this applicant
            var viewModel = applicant.LoanApplications.Select(app => new ApplicantViewModel
            {
                ApplicationId = app.applicationID,
                SubmissionDate = app.dob, // Change this if actual submission date exists
                Status = app.LoanStatu?.loanStatus ?? "Pending",

                RegistrationId = app.registrationID.ToString(),
                FullName = applicant.fullName,
                Email = applicant.username,
                Address = app.address,
                AadhaarNumber = app.adharNum,
                PANNumber = app.panNum,
                DOB = app.dob,
                MonthlyIncome = app.monthlyIncome,
                CompanyName = app.companyName
            }).ToList();

            return View(viewModel);
        }

        public ActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Apply(ApplicantViewModel model)
        {
            var registrationId = _db.Applicants.FirstOrDefault(u => u.username == model.Email).registrationID;
            if (ModelState.IsValid)
            {

                LoanApplication application = new LoanApplication
                {

                    email = model.Email,
                    address = model.Address,
                    adharNum = model.AadhaarNumber,
                    panNum = model.PANNumber,
                    dob = model.DOB,
                    monthlyIncome = model.MonthlyIncome,
                    companyName = model.CompanyName,
                    registrationID = registrationId
                };

                _db.LoanApplications.Add(application);
                _db.SaveChanges();

                LoanStatu loanStatus = new LoanStatu
                {
                    LoanApplication = application,
                    registrationID = registrationId,
                    loanStatus = "Pending", // Default value
                    remark = "Application submitted"
                };

                _db.LoanStatus.Add(loanStatus);
                _db.SaveChanges();
                return RedirectToAction("ApplicantDashboard", "Applicant");
            }
            return View(model);
        }



        // Edit profile
        public ActionResult Edit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Edit(ApplicantViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Update applicant info
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            int userId = Convert.ToInt32(Session["userId"]);
            var user = _db.Applicants.Find(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (model.CurrentPassword != user.password)
            {
                ModelState.AddModelError("", "Current password is incorrect.");
                return View(model);
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                ModelState.AddModelError("", "New password and confirmation do not match.");
                return View(model);
            }

            // Save new password (Note: You should hash passwords in production)
            user.password = model.NewPassword;
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            return RedirectToAction("ApplicantDashboard", "Applicant");
        }



        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModelView model)
        {
            if (ModelState.IsValid)
            {
                var user = _db.Applicants.FirstOrDefault(u => u.username == model.username && u.password == model.password);
                if (user != null )
                {
                    Session["userId"] = user.registrationID;
                    Session["username"] = user.username;
                    Session["userpass"] = user.password;
                    Session["name"] = user.fullName;
                    
                    return RedirectToAction("ApplicantDashboard", "Applicant");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "User doesn't exists");
            return View(model);
        }
    }
}