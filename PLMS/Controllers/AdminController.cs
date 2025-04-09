using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PLMS.Controllers
{
    public class AdminController : Controller
    {
        // GET: User
        public ActionResult Admin()
        {
            return View();
        }

        public ActionResult AddNew()
        {

            return View();
        }

        public ActionResult ViewApplicants()
        {
            return View();
        }
        public ActionResult EditAccess()
        {
            return View();
        }
    }
}