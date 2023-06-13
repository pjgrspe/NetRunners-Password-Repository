using PasswordRepository.Helpers;
using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;

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
                user.DEACT_DATE = DateTime.Now;
                user.EXPIRY_DATE = DateTime.Now.AddMonths(3); //PRODUCTION CODE
                //user.EXPIRY_DATE = DateTime.Now.AddSeconds(15); //TEST CODE

                entities.SaveChanges();
                //Success Message
                //clears all Session variables
                Session["ID"] = null;
                Session["UserName"] = null;
                Session["PIN"] = null;
                Session["TO"] = null;
                Session["timedout"] = null;
                Session["Status"] = null;
                Session["Access"] = null;

                //Clears whole session altogether
                Session.Abandon();

                //Clears cookie session
                FormsAuthentication.SignOut();

                //Brings back to home
                return RedirectToAction("Index", "Home");

            }
        }


        public ActionResult UpdateUser(string userFN, string userLN, string userUN, string userPW, string userEMAIL)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var userID = (int)Session["ID"];
                var userLogin = entities.TBL_LOGIN.Where(x => x.ID == userID).FirstOrDefault();
                var userDetails = entities.TBL_USER_DETAILS.Where(x => x.UID == userID).FirstOrDefault();

                //If entry login and details are not found (should be impossible)
                if (userLogin == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }
                if (userDetails == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }

                //Encrypts entered password
                var EncryptedPass = Encrypter.EncryptString(userPW);

                //Sets selected entry's data to the information passed through the function
                userLogin.USERNAME = userUN;
                userLogin.PASSWORD = EncryptedPass;
                userLogin.EMAIL = userEMAIL;
                userDetails.FIRSTNAME = userFN;
                userDetails.LASTNAME = userLN;
                userDetails.DATE_MODIFIED = DateTime.Now;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "User entry updated" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        public ActionResult UpdatePIN(string userPIN, int userTO)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to UID
                var userID = (int)Session["ID"];
                var userDetails = entities.TBL_USER_DETAILS.Where(x => x.UID == userID).FirstOrDefault();

                //If entry details are not found (should be impossible)
                if (userDetails == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }

                //Encrypts entered password
                var EncryptedPIN = Encrypter.EncryptString(userPIN);

                //Sets selected entry's data to the information passed through the function
                userDetails.PIN = EncryptedPIN;
                userDetails.TIMEOUT = userTO;
                Session["TO"] = userDetails.TIMEOUT;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "User entry updated" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

    }
}