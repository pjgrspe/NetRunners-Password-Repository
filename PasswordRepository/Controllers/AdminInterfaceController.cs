using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class AdminInterfaceController : Controller
    {
        // GET: AdminInterface
        [Authorize]
        //Cache magic code, code from breaking when pressing back button
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // Runs the code on page load
        public ActionResult Index()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //Checks if user is currently timed out, redirects to pin interface index if so.
            if ((bool)Session["timedout"] == true)
            {
                return RedirectToAction("index", "PINInterface");
            }

            if ((string)Session["PIN"] == "Undefined")
            {
                return RedirectToAction("index", "PINRegistration");
            }

            if ((bool)Session["Status"] == false)
            {
                return RedirectToAction("deactivated", "Account");
            }

            if ((bool)Session["Access"] == false)
            {
                return RedirectToAction("index", "Dashboard");
            }


            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())

            {
                //For table display
                var LoginDetails = entities.TBL_LOGIN.ToList();
                var Details = entities.TBL_USER_DETAILS.ToList();
                var UserModel = new PasswordEntryModel()
                {
                    UserCredList = LoginDetails,
                    UserDetailsList = Details,
                    Password = new TBL_PASSWORD_REPO()
                };


                //Returns the set model for display
                return View(UserModel);
            }
        }
    }
}