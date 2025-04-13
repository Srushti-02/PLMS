using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLMS.Models.DbModel;
using PLMS.Models.ModelViews;

namespace PLMS.Controllers
{
    
    public class ApplicantController : Controller
    {
        TrainingProjectEntities1 _db = new TrainingProjectEntities1();
        // GET: Applicant
        public ActionResult ApplicantDashboard()
        {
            return View();
        }

        // Apply 
        public ActionResult Apply()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Apply(ApplicantViewModel model)
        {
            if (ModelState.IsValid)
            {
                LoanApplication application = new LoanApplication
                {
                    fullName = model.FullName,
                    email = model.Email,
                    address = model.Address,
                    adharNum = model.AadhaarNumber,
                    panNum = model.PANNumber,
                    dob = model.DOB,
                    monthlyIncome = model.MonthlyIncome,
                    companyName = model.CompanyName,
                    
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
    }
}