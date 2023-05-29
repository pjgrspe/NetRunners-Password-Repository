using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class PINInterfaceController : Controller
    {
        // GET: PINInterface
        // Runs the code on page load
        public ActionResult Index()
        {
            //Checks if there is session in place, redirects to dashboard index if there is one already
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }
            //Sets the session's timedout variable to once the page loads
            Session["timedout"] = true;
            return View();
            
        }


        //Basic login check
        [HttpPost]
        public ActionResult PinValidate(string pinCode)
        {
            // Validates if the entered PIN matches the user's pin
            if (pinCode == "1234") //Temporary code
            //if (pinCode == (string)Session["PIN"]) //PROD code
            {
                //Resets the timedout variable to false then redirects to dashboard
                Session["timedout"] = false;
                return RedirectToAction("Index", "Dashboard");
            }
            else
            {
                //Throws an error code if PIN doesnt match
                ViewBag.ErrorPIN = "PIN invalid!";
                return View("Index");
            }
        }
    }

    
}