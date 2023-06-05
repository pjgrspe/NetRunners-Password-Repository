using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PasswordRepository.Controllers
{
    public class AccountController : Controller
    {
        // GET: Deactivated
        public ActionResult Deactivated()
        {
            if ((bool)Session["Status"] == true)
            {
                return RedirectToAction("index", "dashboard");
            }

            return View();
        }

        public ActionResult Deleted()
        {
            return View();
        }

        public ActionResult Reactivate()
        {
            using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            {
                //Sets password variable to the table entry that matches the given password ID to PID
                var currentID = (int)Session["ID"];
                var user = entities.TBL_LOGIN.Where(x => x.ID == currentID).FirstOrDefault();
                //If entry not found (should be impossible)
                if (user == null)
                {
                    return View("Deactivated");
                }

                //Updates the entry's data, flags the trash variable, updates the delete date and adds expiry date which is set to 1 month from now
                user.STATUS = true;
                Session["Status"] = true;
                //DEBUG CODE// Password.EXPIRY_DATE = DateTime.Now.AddSeconds(15);

                //Brings back to home
                return RedirectToAction("Index", "Dashboard");

            }
        }
    }
}