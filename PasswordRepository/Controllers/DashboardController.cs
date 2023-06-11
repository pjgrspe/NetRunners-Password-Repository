using PasswordRepository.Helpers;
using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Authorize]
        //Cache magic code, code from breaking when pressing back button
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // Runs the code on page load
        public ActionResult Index() // FOR MAIN PROD
        {
            //Checks if there is session in place, redirects to login index if there is none.
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

            if ((bool)Session["Access"] == true)
            {
                return RedirectToAction("index", "AdminInterface");
            }

            //Sets the entity object
            //To display all the passwords for the specific user
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //For table display
                var data = entities.TBL_PASSWORD_REPO.ToList();
                var PasswordModel = new PasswordEntryModel()
                {
                    Passwords = data,
                    Password = new TBL_PASSWORD_REPO()
                };

                //For deleting due trash
                var startDate = DateTime.Now;
                var currentUser = (int)Session["ID"];
                var eData = entities.TBL_PASSWORD_REPO.Where(x => x.EXPIRY_DATE <= startDate && x.UID.Equals(currentUser)).ToList();
                foreach (var PassEntry in eData)
                {
                    PassEntry.isActive = false;
                    entities.SaveChanges();
                }

                //Returns the set model for display
                return View(PasswordModel);
            }
        }

        //TEST PAGE
        [Authorize]
        public ActionResult Test() // FOR DEBUGGING
        {
            //Checks if there is session in place, redirects to login index if there is none.
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                var data = entities.TBL_PASSWORD_REPO.ToList();

                var PasswordModel = new PasswordEntryModel()
                {
                    Passwords = data,
                    Password = new TBL_PASSWORD_REPO()
                };

                return View(PasswordModel);
            }
        }

        [HttpPost]
        //Main function for inserting a password entry
        public ActionResult Entry(PasswordEntryModel model)
        {
            //Checks if model is valid
            if (ModelState.IsValid)
            {
                //Set entity object
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    //Encrypts entered password
                    var ePassword = Encrypter.EncryptString(model.textbox_PR_PASSWORD);
                    //New data container with the entered data, packages data for sending.
                    var newUData = new TBL_PASSWORD_REPO
                    {
                        UID = (int)Session["ID"],
                        PR_TITLE = model.textbox_PR_TITLE,
                        PR_EMAIL = model.textbox_PR_EMAIL,
                        PR_USERNAME = model.textbox_PR_USERNAME,
                        PR_PASSWORD = ePassword,
                        PR_URL = model.textbox_PR_URL,
                        PR_NOTES = model.textbox_PR_NOTES,
                        isTrashed = false,
                        isActive = true,
                        ENTRY_CREATED = DateTime.Now,
                        freq = 0
                    };
                    //Sends data to the database
                    entities.TBL_PASSWORD_REPO.Add(newUData);
                    entities.SaveChanges();

                }
                //Redirects to dashboard index
                return RedirectToAction("Index", "Dashboard");
            }


            //Viewbag error for when the model is invalid
            ViewBag.ErrorMessage = "Invalid";
            return View();
        }

        [HttpPost]
        //Main function for deleting an entry
        public ActionResult DeleteEntry(int passwordID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                //Updates the entry's data, flags the trash variable, updates the delete date and adds expiry date which is set to 1 month from now
                Password.isTrashed = true;
                Password.ENTRY_DELETED = DateTime.Now;
                Password.EXPIRY_DATE = DateTime.Now.AddMonths(1);

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        //Main function for updating an entry
        [HttpPost]
        public ActionResult UpdateEntry(int passwordID, string passwordTitle, string passwordEmail, string passwordUname, string passwordPassword, string passwordURL, string passwordNotes)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }

                //Encrypts entered password
                var EncryptedPass = Encrypter.EncryptString(passwordPassword);

                //Sets selected entry's data to the information passed through the function
                Password.PR_TITLE = passwordTitle;
                Password.PR_EMAIL = passwordEmail;
                Password.PR_USERNAME = passwordUname;
                Password.PR_PASSWORD = EncryptedPass;
                Password.PR_URL = passwordURL;
                Password.PR_NOTES = passwordNotes;

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

        //TRASHBOARD PAGE
        public ActionResult Trashboard() // FOR TESTING THE TRASH TABLE
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if ((string)Session["PIN"] == "Undefined")
            {
                return RedirectToAction("index", "PINRegistration");
            }

            if ((bool)Session["timedout"] == true)
            {
                return RedirectToAction("index", "PINInterface");
            }

            if ((bool)Session["Status"] == false)
            {
                return RedirectToAction("deactivated", "Account");
            }

            if ((bool)Session["Access"] == true)
            {
                return RedirectToAction("index", "AdminInterface");
            }

            //Sets the entity object
            //To display all the deleted passwords for the specific user
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                var data = entities.TBL_PASSWORD_REPO.ToList();
                var PasswordModel = new PasswordEntryModel()
                {
                    Passwords = data,
                    Password = new TBL_PASSWORD_REPO()
                };

                //Returns the set model for display
                return View(PasswordModel);
            }
        }

        //[TRASHBOARD FUNCTION] Restores deleted entry
        [HttpPost]
        public ActionResult RestoreEntry(int passwordID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                //Updates the entry's data, updates the trashed and active variables, clears the delete and expiry dates
                Password.isTrashed = false;
                Password.isActive = true;
                Password.ENTRY_DELETED = null;
                Password.EXPIRY_DATE = null;


                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        //[TRASHBOARD FUNCTION] Permanently deletes the deleted password
        [HttpPost]
        public ActionResult PermDeleteEntry(int passwordID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                //Updates the entry's data, updates the active variable, sets the expiry date to the present time and date.
                Password.isActive = false;
                Password.EXPIRY_DATE = DateTime.Now;


                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    //Success Message
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    //Error Message
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        //[TRASHBOARD FUNCTION] For clearing the entire trashboard
        [HttpPost]
        public ActionResult DeleteAllEntry(int userID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.UID.Equals(userID) && x.isTrashed.Equals(true) && x.isActive.Equals(true));
                //If there are no entries found, returns an error message
                if (Password == null)
                {
                    return Json(new { msg = "Table already empty" });
                }
                else
                {
                    //Loops for every entries in the list query
                    foreach (var PasswordEntry in Password)
                    {
                        //updates isActive flag for every entry to false, updates the expiry date to now
                        PasswordEntry.isActive = false;
                        PasswordEntry.EXPIRY_DATE = DateTime.Now;
                    }

                    //Saves the changes
                    if (entities.SaveChanges() >= 1)
                    {
                        //Success Message
                        return Json(new { msg = "Entries deleted" });
                    }
                    else
                    {
                        //Error Message
                        return Json(new { msg = "An error occurred(Controller)" });
                    }
                }

            }
        }

        public ActionResult RestoreAllEntry(int userID)
        {
            //Sets the entity object
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.UID.Equals(userID) && x.isTrashed.Equals(true) && x.isActive.Equals(true));
                //If there are no entries found, returns an error message
                if (Password == null)
                {
                    return Json(new { msg = "Table already empty" });
                }
                else
                {
                    //Loops for every entries in the list query
                    foreach (var PasswordEntry in Password)
                    {
                        //updates isActive flag for every entry to false, updates the expiry date to now
                        PasswordEntry.isTrashed = false;
                        PasswordEntry.isActive = true;
                        PasswordEntry.ENTRY_DELETED = null;
                        PasswordEntry.EXPIRY_DATE = null;
                    }

                    //Saves the changes
                    if (entities.SaveChanges() >= 1)
                    {
                        //Success Message
                        return Json(new { msg = "Entries Restored!" });
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
}