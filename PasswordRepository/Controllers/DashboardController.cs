using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    //[Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Authorize]
        public ActionResult Index() // FOR MAIN PROD
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
        [Authorize]
        public ActionResult Test() // FOR DEBUGGING
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            return View();
        }
    }
}