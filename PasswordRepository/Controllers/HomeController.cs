using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class HomeController : Controller
    {
        //Launches index homepage
        public ActionResult Index()
        {
            ViewBag.Message = "Your home page.";

            return View();
        }

        //Launches About homepage
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        //Launches Contact homepage
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}