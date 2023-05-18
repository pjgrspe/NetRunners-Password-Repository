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
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult PinValidate(string pinCode)
        {
            // Perform validation of the entered PIN code
            // if (pinCode == "1234") // Replace "1234" with your actual PIN code
            if (pinCode == "1234")
            {
                // PIN code is correct, allow the user to proceed
                // You can redirect to a specific page or the default page
                return RedirectToAction("Index", "Home");
            }
            else
            {
                // PIN code is incorrect, display an error message and redirect back to the PIN code entry page
                ModelState.AddModelError("", "Invalid PIN code. Please try again.");
                return View("Index");
            }
        }
    }

    
}