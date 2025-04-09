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
        Models.TrainingProjectEntities _db  = new Models.TrainingProjectEntities();
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
                    else return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Wrong username or password");
                }

            }
            return View(model);
        }
        public ActionResult SignUP()
        {
            return View();
        }
    }
}