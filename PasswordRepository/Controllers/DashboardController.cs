using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult Index() // FOR MAIN PROD
            {
                return View();
            }

        //public ActionResult Test() // FOR DEBUGGING
        //{
          //  return View();
        //}
    }
}