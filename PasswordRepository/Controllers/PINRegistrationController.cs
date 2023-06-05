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

            if ((bool)Session["Status"] == false)
            {
                return RedirectToAction("deactivated", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult RegisterPIN(string regPIN, string regREPEATPIN, int regTIMEOUT) 
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                if (!regPIN.Equals(regREPEATPIN))
                {
                    //ViewBag.ErrorPIN = "PIN does not match!";
                    return Json(new { msg = "PIN does not match!", success = false} );
                }


                var CurrentUser = (int)Session["ID"];
                //Sets password variable to the table entry that matches the given password ID to PID
                var UserData = entities.TBL_USER_DETAILS.Where(x => x.UID == CurrentUser).FirstOrDefault();
                //If entry not found (should be impossible)
                if (UserData == null)
                {
                    //ViewBag.ErrorPIN = "User doesn't exist (how did you get here?)";
                    return Json(new { msg = "User doesn't exist (how did you get here?)", success = false });
                }

                //Encrypts entered password
                var PINstring = regPIN;
                var EncryptedPIN = Encrypter.EncryptString(PINstring);

                //Sets selected entry's data to the information passed through the function
                UserData.PIN = EncryptedPIN;
                UserData.TIMEOUT = regTIMEOUT;
                Session["PIN"] = UserData.PIN;
                Session["TO"] = UserData.TIMEOUT;

                //Saves the changes
                if (entities.SaveChanges() >= 1)
                {
                    Session["PIN"] = UserData.PIN;
                    Session["TO"] = UserData.TIMEOUT;

                    //ViewBag.SuccessPIN = "PIN Registered!";

                    //Success Message
                    return Json(new { msg = "PIN Registered!", success = true });
                }
                else
                {
                    //Error Message
                    //ViewBag.ErrorPIN = "An error occured (Controller)";
                    return Json(new { msg = "An error occurred(Controller)", success = false });
                }

            }
        }
    }
}