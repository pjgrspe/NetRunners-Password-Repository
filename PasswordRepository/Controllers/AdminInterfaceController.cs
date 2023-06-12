using PasswordRepository.Helpers;
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

        [HttpPost]
        //Main function for inserting a password entry
        public ActionResult UserEntry(PasswordEntryModel model)
        {
            //Checks if model is valid
            if (ModelState.IsValid)
            {
                //Set entity object
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    //Sets a variable data that checks if the email already exists in the database
                    var eData = entities.TBL_LOGIN.Where(x => x.EMAIL.Equals(model.textbox_UE_EMAIL)).FirstOrDefault();
                    //Errors if email already exists
                    if (eData != null)
                    {
                        ViewBag.Error = "Email already exists!";
                        return View("Index");
                    }

                    //Sets a variable data that checks if the username already exists in the database
                    var uData = entities.TBL_LOGIN.Where(x => x.USERNAME.Equals(model.textbox_UE_USERNAME)).FirstOrDefault();
                    //Errors if username already exists
                    if (uData != null)
                    {
                        ViewBag.Error = "Username already exists!";
                        return View("Index");
                    }

                    //Encrypts entered password
                    var ePassword = Encrypter.EncryptString(model.textbox_UE_PASSWORD);
                    //New data container with the entered data, packages data for sending.
                    var newUData = new TBL_LOGIN
                    {
                        USERNAME = model.textbox_UE_USERNAME,
                        EMAIL = model.textbox_UE_EMAIL,
                        PASSWORD = ePassword,
                        STATUS = true,
                        DATE_CREATED = DateTime.Now,
                        ACCESSLVL = false
                    };
                    //Sends data to the database
                    entities.TBL_LOGIN.Add(newUData);

                    var newUDetail = new TBL_USER_DETAILS
                    {
                        UID = newUData.ID,
                        FIRSTNAME = model.textbox_UE_FNAME,
                        LASTNAME = model.textbox_UE_FNAME,
                        DATE_CREATED = DateTime.Now,
                        DATE_MODIFIED = DateTime.Now,
                        PIN = "Undefined",
                        TIMEOUT = -1
                    };
                    entities.TBL_USER_DETAILS.Add(newUDetail);
                    entities.SaveChanges();

                }
                //Redirects to dashboard index
                return RedirectToAction("Index", "AdminInterface");
            }


            //Viewbag error for when the model is invalid
            ViewBag.ErrorMessage = "Invalid";
            return View("Index");
        }

        public ActionResult BanUser(int userID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_LOGIN.Where(x => x.ID == userID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                //Updates the entry's data, flags the trash variable, updates the delete date and adds expiry date which is set to 1 month from now
                Password.STATUS = false;
                Password.DEACT_DATE = DateTime.Now;
                Password.EXPIRY_DATE = DateTime.Now;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "User banned" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        public ActionResult UnbanUser(int userID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_LOGIN.Where(x => x.ID == userID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                //Updates the entry's data, flags the trash variable, updates the delete date and adds expiry date which is set to 1 month from now
                Password.STATUS = true;
                Password.DEACT_DATE = null;
                Password.EXPIRY_DATE = null;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "User banned" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        public ActionResult UpdateEntry(int userID, string userFname, string userLname, string userUsername, string userEmail, string userPIN, int userTO, bool userStatus)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var UserLogin = entities.TBL_LOGIN.Where(x => x.ID == userID).FirstOrDefault();
                var UserDetail = entities.TBL_USER_DETAILS.Where(x => x.UID == userID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (UserLogin == null || UserDetail == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }

                //Encrypts entered password
                var EncryptedPIN = Encrypter.EncryptString(userPIN);

                //Sets selected entry's data to the information passed through the function
                UserDetail.FIRSTNAME = userFname;
                UserDetail.LASTNAME = userLname;
                UserLogin.USERNAME = userUsername;
                UserLogin.EMAIL = userEmail;
                UserDetail.PIN = EncryptedPIN;
                UserDetail.TIMEOUT = userTO;
                UserLogin.STATUS = userStatus;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "Entry updated" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        public ActionResult UpdatePassUserEntry(int userID, string userPassword)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var UserLogin = entities.TBL_LOGIN.Where(x => x.ID == userID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (UserLogin == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }

                //Encrypts entered password
                var EncryptedPASS = Encrypter.EncryptString(userPassword);

                //Sets selected entry's data to the information passed through the function
                UserLogin.PASSWORD = EncryptedPASS;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "Entry updated" });
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