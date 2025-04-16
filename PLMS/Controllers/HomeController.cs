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
        G1IBMDbEntities _db = new G1IBMDbEntities();

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
                if (user != null)
                {
                    if (user.access == "Disabled")
                    {
                        ViewBag.ErrorMessage = "Your access has been disabled.";
                        return View(model);
                    }

                    Session["username"] = user.username;
                    Session["userID"] = user.userID;

                    if(user.role ==  "Admin") return RedirectToAction("Admin", "Admin");
                    else if(user.role == "LO") return RedirectToAction("Dashboard", "Officer");
                    else if(user.role == "LI") return RedirectToAction("Dashboard", "LoanInspector");
                }
                ViewBag.ErrorMessage = "Invalid username or password.";
                return View(model);
            }

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
                var existingApplicant = _db.Applicants.FirstOrDefault(a => a.username == model.Email);
                if (existingApplicant != null)
                {
                    ModelState.AddModelError("Email", "An account with this email already exists.");
                    return View(model);
                }

                var newApplicant = new Applicant
                {
                    fullName = model.FullName,
                    username = model.Email,
                    password = model.Password,
                    phoneNum = model.ContactNumber
                };

                try
                {
                    _db.Applicants.Add(newApplicant);
                    _db.SaveChanges();

                    return RedirectToAction("Login", "Applicant");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = "An error occurred during Sign Up" + ex;
                    return View(model);
                }
            }

            ViewBag.ErrorMessage = "Signup failed. Please check the form and try again.";
            return View(model);
        }

        public ActionResult LogOut()
        {
            Session.Clear();
            TempData["SuccessMessage"] = "You have been logged out.";
            return RedirectToAction("Login", "Home");
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

            int userId = Convert.ToInt32(Session["userID"]);
            var user = _db.USERs.Find(userId);

            if (user == null)
            {
                ModelState.AddModelError("", "User not found.");
                return View(model);
            }

            if (model.CurrentPassword != user.userpass)
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
            user.userpass = model.NewPassword;
            _db.Entry(user).State = EntityState.Modified;
            _db.SaveChanges();

            TempData["SuccessMessage"] = "Password changed successfully!";
            LogOut();
            return RedirectToAction("Login", "Home");
        }


    }
}