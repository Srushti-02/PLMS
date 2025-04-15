﻿using System;
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
                    access = "Enabled",
                };

                _db.USERs.Add(user);
                _db.SaveChanges();

                return RedirectToAction("Admin", "Admin");
            }
            return View(model);
        }

        public ActionResult ViewApplications()
        {
            var applications = _db.LoanApplications.Include("Applicant").Include("LoanStatu").Include("Officers.USER").ToList();
            var officers = _db.USERs.Where(u => (u.role == "LO" || u.role == "LI") && u.access!="Disabled").ToList();

            var viewModel = applications.Select(app => new AdminApplicationViewModel
            {
                ApplicationId = app.applicationID,
                ApplicantName = app.Applicant.fullName,
                Status = app.LoanStatu?.loanStatus,
                Email = app.email,
                Address = app.address,
                PANNumber = app.panNum,
                AadhaarNumber = app.adharNum,
                CompanyName = app.companyName,
                MonthlyIncome = app.monthlyIncome,

                AssignedOfficerId = app.Officers.FirstOrDefault()?.officerID,
                AssignedOfficerName = app.Officers.FirstOrDefault()?.USER?.fullName,
                OfficersList = officers.Select(o => new SelectListItem
                {
                    Value = o.userID.ToString(),
                    Text = o.fullName + " (" + o.role + ")"
                })
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