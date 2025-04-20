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
using System.Data.Entity.Migrations.Infrastructure;

namespace PLMS.Controllers
{
    
    public class AdminController : Controller
    {
        G1IBMDbEntities _db = new G1IBMDbEntities();
        // GET: User

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Session["username"] == null)
            {
                // Redirect to login page if session is not set
                filterContext.Result = RedirectToAction("Login", "Home");
            }

            string userRole = Session["role"] as string;
            if (Session["username"]!=null && userRole != "Admin")
            {
                filterContext.Result = RedirectToAction("Login", "Home");
            }

            base.OnActionExecuting(filterContext);
        }
        
        public ActionResult Admin()
        {

            return View();
        }



        public ActionResult AddNew()
        {

            return View();
        }
        //[HttpPost]
        //public ActionResult AddNew(AdminViewModel model)
        //{
        //    var officer = _db.USERs.FirstOrDefault(u => u.username == model.username);
        //    if(officer != null)
        //    {
        //        ModelState.AddModelError("", "User already exists");
        //        return View(model);
        //    }
        //    if (ModelState.IsValid)
        //    {
        //        var user = new USER
        //        {
        //            username = model.username,
        //            userpass = model.userpass,
        //            fullName = model.fullName,
        //            phoneNum = model.phoneNum,
        //            role = model.role,
        //            access = "Enabled",
        //        };

        //        _db.USERs.Add(user);
        //        _db.SaveChanges();

        //        return RedirectToAction("Admin", "Admin");
        //    }
        //    return View(model);
        //}
        [HttpPost]

        public ActionResult AddNew(AdminViewModel model)
        {
            var existingUser = _db.USERs.FirstOrDefault(u => u.username == model.username);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "User already exists";
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Generate random password
                //string generatedPassword = GenerateSecurePassword(10); // adjust length if needed

                var user = new USER
                {
                    username = model.username,
                    //userpass = generatedPassword, // Store auto-generated password
                    userpass = "123",
                    fullName = model.fullName,
                    phoneNum = model.phoneNum,
                    role = model.role,
                    access = "Enabled",
                };

                _db.USERs.Add(user);
                _db.SaveChanges();

                //TempData["SuccessMessage"] = $"User added! Temporary password: {generatedPassword}";
                TempData["SuccessMessage"] = $"User Added! Temporary password: {user.userpass}";
                return View(model);
            }
            
            return View(model);
        }
        //private string GenerateSecurePassword(int length)
        //{
        //    const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
        //    var res = new char[length];
        //    var rnd = new Random();

        //    for (int i = 0; i < length; i++)
        //    {
        //        res[i] = valid[rnd.Next(valid.Length)];
        //    }

        //    return new string(res);
        //}


        public ActionResult ViewApplications()
        {
            var applications = _db.LoanApplications
                .Include("Applicant")
                .Include("LoanStatu")
                .Include("Officers.USER")
                .ToList();

            var officers = _db.USERs
                .Where(u => (u.role == "LO" || u.role == "LI") && u.access != "Disabled")
                .ToList();

            var filteredApplications = applications
                .Where(app =>
                {
                    var status = app.LoanStatu?.loanStatus ?? "Pending";
                    var officerRoles = app.Officers.Select(o => o.USER.role).ToList();
                    var remark = app.LoanStatu?.remark ?? "Application submitted and edited";
                    bool hasLO = officerRoles.Contains("LO");
                    bool hasLI = officerRoles.Contains("LI");
                    bool isPending = status == "Pending";
                    bool isApproved = status == "Approved";
                    bool isRemark = remark == "Application submitted and edited";

                    // CASE 1: New application (Pending + no LO assigned)
                    bool showForLO = isPending && !hasLO;
                    
                    // CASE 2: Approved by LO, waiting for LI
                    bool showForLI = isApproved && hasLO && !hasLI;

                    return showForLO || showForLI;
                })
                .ToList();




            var viewModel = filteredApplications.Select(app =>
            {
                var latestOfficer = app.Officers
                    .OrderByDescending(o => o.officerID)
                    .FirstOrDefault();

                return new AdminApplicationViewModel
                {
                    ApplicationId = app.applicationID,
                    ApplicantName = app.Applicant.fullName,
                    Status = app.LoanStatu?.loanStatus ?? "Pending",
                    Remark = app.LoanStatu?.remark,
                    Email = app.email,
                    Address = app.address,
                    PANNumber = app.panNum,
                    AadhaarNumber = app.adharNum,
                    CompanyName = app.companyName,
                    MonthlyIncome = app.monthlyIncome,

                    AssignedOfficerId = latestOfficer?.officerID,
                    AssignedOfficerName = latestOfficer != null
                        ? latestOfficer.USER?.fullName + " (" + latestOfficer.USER?.role + ")"
                        : "Not Assigned",

                    OfficersList = officers
                    .Where(o =>
                        (app.LoanStatu?.loanStatus == "Pending" && o.role == "LO") ||
                        ((app.LoanStatu?.loanStatus == "Approved") && o.role == "LI")
                    )
                    .Select(o => new SelectListItem
                    {
                        Value = o.userID.ToString(),
                        Text = o.fullName + " (" + o.role + ")"
                    })
                };
            }).ToList();

            return View(viewModel);
        }




        [HttpPost]
        public ActionResult AssignOfficer(int applicationId, FormCollection form)
        {
            string officerKey = "officerId_" + applicationId;
            int officerId = int.Parse(form[officerKey]);

            var officer = _db.USERs.FirstOrDefault(o => o.userID == officerId);
            if (officer != null)
            {
                _db.Officers.Add(new Officer
                {
                    userID = officer.userID,
                    applicationID = applicationId
                });
                _db.SaveChanges();
            }

            return RedirectToAction("ViewApplications");
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