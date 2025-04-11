using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
                if (user != null)
                {
                    Session["username"] = user.username;
                    if (user.role == "Admin") return RedirectToAction("Admin", "Admin");
                    else if (user.role == "LO") return RedirectToAction("LODashboard", "LoanOfficer");
                    else if (user.role == "LI") return RedirectToAction("LIDashboard", "LoanOfficer");
                    else return RedirectToAction("Index", "Home");
                }

            }

            ModelState.AddModelError("", "User doesn't exists");
            return View(model);
        }
        public ActionResult SignUP()
        {
            return View();
        }
    }
}