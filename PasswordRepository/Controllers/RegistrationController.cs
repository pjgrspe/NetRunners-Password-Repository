using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Xml.Linq;
using PasswordRepository.Helpers;
using PasswordRepository.Models;

namespace PasswordRepository.Controllers
{
    public class RegistrationController : Controller
    {
        // GET: Registration
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Index(RegistrationModel model)
        {
            if (ModelState.IsValid)
            {
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {

                    if (!model.PASSWORD.Equals(model.REPEAT_PASSWORD))
                    {
                        ViewBag.Error = "Password does not match!";
                        return View();
                    }

                    var eData = entities.TBL_LOGIN.Where(x => x.EMAIL.Equals(model.EMAIL)).FirstOrDefault();

                    if (eData != null)
                    {
                        ViewBag.Error = "Email already exists!";
                        return View();
                    }

                    var uData = entities.TBL_LOGIN.Where(x => x.USERNAME.Equals(model.USERNAME)).FirstOrDefault();

                    if (uData != null)
                    {
                        ViewBag.Error = "Username already exists!";
                        return View();
                    }

                    var ePassword = Encrypter.Encrypt(model.PASSWORD);

                    var newUData = new TBL_LOGIN
                    {
                        USERNAME = model.USERNAME,
                        PASSWORD = ePassword,
                        EMAIL = model.EMAIL,
                        STATUS = true,
                        DATE_CREATED = DateTime.Now
                    };
                    entities.TBL_LOGIN.Add(newUData);

                    var newUDetail = new TBL_USER_DETAILS
                    {
                        UID = newUData.ID,
                        FIRSTNAME = model.FIRST_NAME,
                        LASTNAME = model.LAST_NAME,
                        DATE_CREATED = DateTime.Now,
                        DATE_MODIFIED = DateTime.Now
                    };
                    entities.TBL_USER_DETAILS.Add(newUDetail);
                    entities.SaveChanges();
                }

                //return RedirectToAction("Index", "Dashboard");  //For Prod
                //return RedirectToAction("Test", "Dashboard");  ///For Test
                return RedirectToAction("Index", "Login");  // Temporary
                //return Redirect("/Content/Index");
            }

            ViewBag.ErrorMessage = "Invalid";
            return View();
        }
    }
}