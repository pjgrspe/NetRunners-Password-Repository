using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PasswordRepository.Helpers;
using PasswordRepository.Models;

namespace PasswordRepository.Controllers
{
    public class PINRegistrationController : Controller
    {
        [Authorize]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // GET: PINRegistration
        public ActionResult Index()
        {
            if (Session["ID"] == null)
            {
                return RedirectToAction("Index", "Login");
            }

            if ((bool)Session["timedout"] == true)
            {
                return RedirectToAction("index", "PINInterface");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPIN(RegistrationModel model) 
        {
            if (ModelState.IsValid)
            {
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    if (!model.textbox_PIN.Equals(model.textbox_REPEAT_PIN))
                    {
                        ViewBag.ErrorPIN = "PIN does not match!";
                        return View("Index");
                    }


                    var CurrentUser = (int)Session["ID"];
                    //Sets password variable to the table entry that matches the given password ID to PID
                    var UserData = entities.TBL_USER_DETAILS.Where(x => x.UID == CurrentUser).FirstOrDefault();
                    //If entry not found (should be impossible)
                    if (UserData == null)
                    {
                        return Json(new { msg = "Entry not found (what?? how???)" });
                    }

                    //Encrypts entered password
                    var PINstring = model.textbox_PIN;
                    var EncryptedPIN = Encrypter.EncryptString(PINstring);

                    //Sets selected entry's data to the information passed through the function
                    UserData.PIN = EncryptedPIN;
                    UserData.TIMEOUT = model.slider_timeout;

                    //Saves the changes
                    if (entities.SaveChanges() >= 1)
                    {
                        Session["PIN"] = UserData.PIN;
                        Session["TO"] = UserData.TIMEOUT;
                        //Success Message
                        return RedirectToAction("Index", "Dashboard");
                    }
                    else
                    {
                        //Error Message
                        ViewBag.ErrorMessage = "An error occured (Controller)";
                    }

                }

            }

            //If model state is invalid, it would throw them back to PIN Registration index with an error message
            ViewBag.ErrorMessage = "Invalid model";
            return View("Index");
        }
    }
}