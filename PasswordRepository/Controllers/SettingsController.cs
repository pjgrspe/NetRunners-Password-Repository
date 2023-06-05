using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PasswordRepository.Controllers
{
    public class SettingsController : Controller
    {
        // GET: Settings
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


            //Sets the entity object
            //To display all the passwords for the specific user
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

        public ActionResult Deactivate()
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var currentID = (int)Session["ID"];
                var user = entities.TBL_LOGIN.Where(x => x.ID == currentID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (user == null)
                {
                    return View("Index");
                }

                //Updates the entry's data, flags the trash variable, updates the delete date and adds expiry date which is set to 1 month from now
                user.STATUS = false;
                //DEBUG CODE// Password.EXPIRY_DATE = DateTime.Now.AddSeconds(15);

                entities.SaveChanges();
                //Success Message
                //clears all Session variables
                Session["ID"] = null;
                Session["UserName"] = null;
                Session["PIN"] = null;
                Session["TO"] = null;
                Session["timedout"] = null;

                //Clears whole session altogether
                Session.Abandon();

                //Clears cookie session
                FormsAuthentication.SignOut();

                //Brings back to home
                return RedirectToAction("Index", "Home");

            }
        }
    }
}