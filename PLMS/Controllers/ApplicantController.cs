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
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        { 

            var response = filterContext.HttpContext.Response;
            response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            response.Cache.SetValidUntilExpires(false);
            response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            response.Cache.SetCacheability(HttpCacheability.NoCache);
            response.Cache.SetNoStore();

            base.OnActionExecuting(filterContext);
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
                if (user != null)
                {
                    Session["userId"] = user.registrationID;
                    Session["username"] = user.username;
                    Session["userpass"] = user.password;
                    Session["name"] = user.fullName;
                    TempData["SuccessMessage"] = "Successful Login";
                    return RedirectToAction("ApplicantDashboard", "Applicant");
                }

                ViewBag.ErrorMessage = "Invalid username or password.";
                return View(model);
            }

            ModelState.AddModelError("", "User doesn't exists");
            return View(model);
        }
        
        public ActionResult ApplicantDashboard()
        {
            string username = Session["username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Applicant");
            }

            var applicant = _db.Applicants
                .Include("LoanApplications.LoanStatu") // Include related LoanApplications and LoanStatus
                .FirstOrDefault(a => a.username == username);

            if (applicant == null)
            {
                TempData["FailureMessage"] = "Applicant not found.";
                return RedirectToAction("Login", "Applicant");
            }

            // Prepare ViewModel from the LoanApplications of this applicant
            var viewModel = applicant.LoanApplications.Select(app => new ApplicantViewModel
            {
                ApplicationId = app.applicationID,
                SubmissionDate = app.dob, // Change this if actual submission date exists
                Status = app.LoanStatu?.loanStatus ?? "Pending",
                Remark = app.LoanStatu.remark,
                RegistrationId = app.registrationID.ToString(),
                FullName = applicant.fullName,
                Email = applicant.username,
                Address = app.address,
                AadhaarNumber = app.adharNum,
                PANNumber = app.panNum,
                DOB = app.dob,
                MonthlyIncome = app.monthlyIncome,
                CompanyName = app.companyName,
                AssignedOfficer = _db.Officers.FirstOrDefault(o => o.applicationID == app.applicationID) != null ?
                      "Assigned" : null
            }).ToList();

            return View(viewModel);
        }

        public ActionResult Apply()
        {
            string username = Session["username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Applicant");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Apply(ApplyEditViewModel model)
        {
            string username = Session["username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Applicant");
            }
            if (username != model.Email)
            {
                ModelState.AddModelError("", "Enter correct Email");
            }
            if (ModelState.IsValid)
            {
                var registrationId = _db.Applicants.FirstOrDefault(u => u.username == model.Email).registrationID;
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
            ViewBag.ErrorMessage = "Something went wrong during applying to loan";
            return View(model);
        }
        // Edit profile
        public ActionResult Edit(int id)
        {
            
            var application = _db.LoanApplications.FirstOrDefault(x => x.applicationID == id);

            if (application == null)
                return HttpNotFound();

            // Check if it's editable
            var status = _db.LoanStatus.FirstOrDefault(x => x.applicationID == id);
            var officer = _db.Officers.FirstOrDefault(o => o.applicationID == id);

            if (status != null && status.loanStatus != "Incomplete" && officer != null)
            {
                TempData["FailureMessage"] = "This application cannot be edited.";
                return RedirectToAction("ApplicantDashboard");
            }

            var viewModel = new ApplyEditViewModel
            {
                ApplicationId = application.applicationID,
                Email = application.email,
                Address = application.address,
                AadhaarNumber = application.adharNum,
                PANNumber = application.panNum,
                DOB = application.dob,
                MonthlyIncome = application.monthlyIncome,
                CompanyName = application.companyName
            };

            return View(viewModel);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ApplyEditViewModel model)
        {

            if (!ModelState.IsValid)
                return View(model);

            var application = _db.LoanApplications.FirstOrDefault(x => x.applicationID == model.ApplicationId);

            if (application == null)
                return HttpNotFound();

            var status = _db.LoanStatus.FirstOrDefault(x => x.applicationID == model.ApplicationId);
            var officer = _db.Officers.FirstOrDefault(o => o.applicationID == model.ApplicationId);

            if (status != null && status.loanStatus != "Incomplete" && officer != null)
            {
                TempData["FailureMessage"] = "This application cannot be edited.";
                return RedirectToAction("ApplicantDashboard");
            }

            application.address = model.Address;
            application.adharNum = model.AadhaarNumber;
            application.panNum = model.PANNumber;
            application.dob = model.DOB;
            application.monthlyIncome = model.MonthlyIncome;
            application.companyName = model.CompanyName;
            application.LoanStatu.loanStatus = "Pending";
            application.LoanStatu.remark = "Application submitted and edited";

            _db.Entry(application).State = EntityState.Modified;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Application updated successfully!";
            return RedirectToAction("ApplicantDashboard");
        }


        public ActionResult ChangePassword()
        {
            string username = Session["username"]?.ToString();
            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("Login", "Applicant");
            }
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {

            if (ModelState.IsValid)
            {

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
                LogOut();
                return RedirectToAction("Login", "Applicant");
            }
            ViewBag.ErrorMessage = "Enter correct current password";
            return View(model);
        }


        public ActionResult LogOut()
        {
            Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login", "Applicant");

        }
    }
}