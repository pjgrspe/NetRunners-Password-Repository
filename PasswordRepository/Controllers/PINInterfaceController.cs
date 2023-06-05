using PasswordRepository.Helpers;
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

            if ((string)Session["PIN"] == "Undefined")
            {
                return RedirectToAction("index", "PINRegistration");
            }

            if ((bool)Session["Status"] == false)
            {
                return RedirectToAction("deactivated", "Account");
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
            //if (pinCode == "1234") //Temporary code
            var entryPIN = Encrypter.DecryptString((string)Session["PIN"]);
            if (pinCode == entryPIN) //prod code
            {
                //Resets the timedout variable to false then redirects to dashboard
                Session["timedout"] = false;
                ViewBag.SuccessPIN = "Login Successful!";
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