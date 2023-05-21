using PasswordRepository.Helpers;
using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    //[Authorize]
    public class DashboardController : Controller
    {
        // GET: Dashboard
        [Authorize]
        public ActionResult Index() // FOR MAIN PROD
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if ((bool)Session["timedout"] == true)
            {
                return RedirectToAction("index", "PINInterface");
            }

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

                //returns model
                return View(PasswordModel);
            }
        }
        [Authorize]
        public ActionResult Test() // FOR DEBUGGING
        {
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

        public ActionResult TrashboardTEST() // FOR TESTING THE TRASH TABLE
        {
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

        public ActionResult Entry(PasswordEntryModel model)
        {
            if (ModelState.IsValid)
            {
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    var ePassword = Encrypter.EncryptString(model.textbox_PR_PASSWORD);
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
                    entities.TBL_PASSWORD_REPO.Add(newUData);
                    entities.SaveChanges();

                }
                return RedirectToAction("Index", "Dashboard");
                //return RedirectToAction("FormTest", "Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid";
            return View();
        }
        [HttpPost]
        public ActionResult DeleteEntry(int passwordID)
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();

                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                Password.isTrashed = true;
                Password.ENTRY_DELETED = DateTime.Now;
                Password.EXPIRY_DATE = DateTime.Now.AddMonths(3);
                //DEBUG CODE//Password.EXPIRY_DATE = DateTime.Now.AddSeconds(15);



                if (entities.SaveChanges() >= 1)
                {
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }
        [HttpPost]
        public ActionResult RestoreEntry(int passwordID)
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();

                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                Password.isTrashed = false;
                Password.isActive = true;
                Password.ENTRY_DELETED = null;
                Password.EXPIRY_DATE = null;



                if (entities.SaveChanges() >= 1)
                {
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }
        [HttpPost]
        public ActionResult PermDeleteEntry(int passwordID)
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();

                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (how???)" });
                }

                Password.isActive = false;
                Password.EXPIRY_DATE = DateTime.Now;



                if (entities.SaveChanges() >= 1)
                {
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }

        [HttpPost]
        public ActionResult UpdateEntry(int passwordID, string passwordTitle, string passwordEmail, string passwordUname, string passwordPassword, string passwordURL, string passwordNotes)
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                var Password = entities.TBL_PASSWORD_REPO.Where(x => x.PID == passwordID).FirstOrDefault();

                if (Password == null)
                {
                    return Json(new { msg = "Entry not found (what?? how???)" });
                }

                var EncryptedPass = Encrypter.EncryptString(passwordPassword);

                Password.PR_TITLE = passwordTitle;
                Password.PR_EMAIL = passwordEmail; 
                Password.PR_USERNAME = passwordUname;
                Password.PR_PASSWORD = EncryptedPass;    
                Password.PR_URL = passwordURL;
                Password.PR_NOTES = passwordNotes;

                if (entities.SaveChanges() >= 1)
                {
                    return Json(new { msg = "Entry deleted" });
                }
                else
                {
                    return Json(new { msg = "An error occurred(Controller)" });
                }

            }
        }


    }
}