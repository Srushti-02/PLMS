using System;
using System.Collections.Generic;
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
            return View();
        }
        //public ActionResult ApplicantDashboard()
        //{
        //    var email = Session["Email"]?.ToString();

        //    if (string.IsNullOrEmpty(email))
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    var applicant = _db.Applicants.FirstOrDefault(a => a.username == email);
        //    if (applicant == null)
        //    {
        //        return RedirectToAction("Login", "Home");
        //    }

        //    var applications = _db.LoanApplications
        //                          .Where(app => app.registrationID == applicant.registrationID)
        //                          .ToList();

        //    return View(applications);
        //}
        // Apply 
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
                return RedirectToAction("ApplicantDashboard", "Applicant");
            }
            return View(model);
        }

        // View past applications
        public ActionResult Applications()
        {
            var applications = _db.LoanApplications.ToList();
            return View(applications);
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

        // Change password
        public ActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ChangePassword(ApplicantViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Change password logic
                return RedirectToAction("Index");
            }
            return View(model);
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
                    Session["username"] = user.username;
                    
                    return RedirectToAction("ApplicantDashboard", "Applicant");
                }

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "User doesn't exists");
            return View(model);
        }
    }
}