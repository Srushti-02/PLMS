using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PLMS.Models;
using PLMS.Models.DbModel;
using PLMS.Models.ModelViews;

namespace PLMS.Controllers
{
    public class AdminController : Controller
    {
        Models.DbModel.TrainingProjectEntities1 _db = new Models.DbModel.TrainingProjectEntities1();
        // GET: User

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["username"] == null)
            {
                // Redirect to login page if session is not set
                filterContext.Result = RedirectToAction("Login", "Home");
            }

            base.OnActionExecuting(filterContext);
        }

        public ActionResult Admin()
        {
            
            return View();
        }


        //private void SendWelcomeEmail(string username, string password, string toEmail)
        //{
        //    var fromAddress = new MailAddress("srushtishindkar93@gmail.com", "Srushti Shindkar");
        //    var toAddress = new MailAddress(toEmail);
        //    const string fromPassword = ""; // Use secure secrets in real apps
        //    const string subject = "Welcome to Our System";
        //    string body = $@"
        //        Hello {username},

        //        Your account has been created successfully.

        //        Username: {username}
        //        Password: {password}

        //        Please login at: https://yourwebsite.com/Login

        //        Best regards,
        //        Admin Team
        //    ";

        //    var smtp = new SmtpClient
        //    {
        //        Host = "smtp.gmail.com", // e.g., smtp.gmail.com
        //        Port = 587,
        //        EnableSsl = true,
        //        DeliveryMethod = SmtpDeliveryMethod.Network,
        //        UseDefaultCredentials = false,
        //        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
        //    };

        //    using (var message = new MailMessage(fromAddress, toAddress)
        //    {
        //        Subject = subject,
        //        Body = body
        //    })
        //    {
        //        smtp.Send(message);
        //    }
        //}


        public ActionResult AddNew()
        {

            return View();
        }
        [HttpPost]
        public ActionResult AddNew(AdminViewModel model)
        {
            var officer = _db.USERs.FirstOrDefault(u => u.username == model.username);
            if(officer != null)
            {
                ModelState.AddModelError("", "User already exists");
                return View(model);
            }
            if (ModelState.IsValid)
            {
                var user = new USER
                {
                    username = model.username,
                    userpass = model.userpass,
                    fullName = model.fullName,
                    phoneNum = model.phoneNum,
                    role = model.role,
                    access = model.access,
                };

                _db.USERs.Add(user);
                _db.SaveChanges();
                //try
                //{
                //    SendWelcomeEmail(model.fullName, model.userpass, model.username); // assuming you have model.email
                //}
                //catch (Exception ex)
                //{
                //    // Log or handle email error
                //    ModelState.AddModelError("", "User added, but failed to send email: " + ex.Message);
                //}
                return RedirectToAction("Admin", "Admin");
            }
            return View(model);
        }

        public ActionResult ViewApplicants()
        {
            var applicant = _db.Applicants.ToList();
            return View(applicant);
        }
        public ActionResult EditAccess()
        {
            var user = _db.USERs.Where(u => u.role != "Admin").ToList();
            return View(user);
        }
        [HttpPost]
        public ActionResult EditAccess(int id, string access, AdminViewModel model)
        { 
            var user = _db.USERs.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            user.access = access;
            _db.SaveChanges();

            return RedirectToAction("EditAccess", "Admin");
        }
    }
}