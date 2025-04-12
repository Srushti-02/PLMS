using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PLMS.Models.DbModel;

namespace PLMS.Controllers
{
    public class LoanOfficerController : Controller
    {
        // GET: LoanOfficer

        public ActionResult LODashboard()
        {
            return View();
        }
       
    }
}