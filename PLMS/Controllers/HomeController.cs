using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLMS.Models.DbModel;
using PLMS.Models.ModelViews;
namespace PLMS.Controllers
{
    public class HomeController : Controller
    {
        Models.DbModel.TrainingProjectEntities1 _db = new Models.DbModel.TrainingProjectEntities1();
        // GET: Home
        public ActionResult Index()
        {
            return View();
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
                var user = _db.USERs.FirstOrDefault(u => u.username == model.username && u.userpass == model.password);
                if (user != null && user.access!="Disabled")
                {
                    Session["username"] = user.username;
                    if (user.role == "Admin") return RedirectToAction("Admin", "Admin");
                    else if (user.role == "LO") return RedirectToAction("LODashboard", "LoanOfficer");
                    else if (user.role == "LI") return RedirectToAction("LIDashboard", "LoanOfficer");
                    else return RedirectToAction("Index", "Home");
                }
                else if(user != null && user.access == "Disabled")
                {
                    return RedirectToAction("Index", "Home");
                }
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "User doesn't exists");
            return View(model);
        }
        public ActionResult SignUP()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var applicant = _db.Applicants.FirstOrDefault(u => u.username == model.Email);
                if(applicant != null)
                {
                    ModelState.AddModelError("Email", "User with this email already exists.");
                    return View(model);
                }
                var newApplicant = new Applicant
                {
                    username = model.Email,
                    password = model.Password,
                    phoneNum = model.ContactNumber
                };
                Session["Name"] = model.FullName;
                Session["Email"] = model.Email;
                try
                {
                    _db.Applicants.Add(newApplicant);
                    _db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            System.Diagnostics.Debug.WriteLine("Property: " + validationError.PropertyName + " Error: " + validationError.ErrorMessage);
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                        }
                    }
                    return View(model);
                }

                TempData["SuccessMessage"] = "Signup successful!";
                return RedirectToAction("Login", "Home"); 
            }
            TempData["FailureMessage"] = "Cannot do signup";
            return View(model);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login", "Home");
        }
    }
}