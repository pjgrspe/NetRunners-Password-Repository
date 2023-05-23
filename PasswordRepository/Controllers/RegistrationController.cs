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
        // Runs the code on page load
        public ActionResult Index()
        {
            //Checks if there is session in place, redirects to dashboard index if there is one already
            if (Session["ID"] != null)
            {
                return RedirectToAction("index", "Dashboard");
            }
            
            //Otherwise load the Registration index page
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //Main Registration function
        public ActionResult Register(RegistrationModel model)
        {
            //checks if Model is valid
            if (ModelState.IsValid)
            {
                //Sets the entity object
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    //Viewbag error for when the textbox passwords do not match
                    if (!model.textbox_PASSWORD.Equals(model.textbox_REPEAT_PASSWORD))
                    {
                        ViewBag.Error = "Password does not match!";
                        return View();
                    }

                    //Sets a variable data that checks if the email already exists in the database
                    var eData = entities.TBL_LOGIN.Where(x => x.EMAIL.Equals(model.textbox_EMAIL)).FirstOrDefault();

                    //Errors if email already exists
                    if (eData != null)
                    {
                        ViewBag.Error = "Email already exists!";
                        return View();
                    }

                    //Sets a variable data that checks if the username already exists in the database
                    var uData = entities.TBL_LOGIN.Where(x => x.USERNAME.Equals(model.textbox_USERNAME)).FirstOrDefault();

                    //Errors if username already exists
                    if (uData != null)
                    {
                        ViewBag.Error = "Username already exists!";
                        return View();
                    }


                    //Encrypts the password before sending to database
                    var ePassword = Encrypter.EncryptString(model.textbox_PASSWORD);

                    //Packages the data to a variable with the corresponding given informaton to be sent to TBL_Login specifically
                    var newUData = new TBL_LOGIN
                    {
                        USERNAME = model.textbox_USERNAME,
                        PASSWORD = ePassword,
                        EMAIL = model.textbox_EMAIL,
                        STATUS = true,
                        DATE_CREATED = DateTime.Now
                    };
                    //Adds a new entry in the TBL login with the appropriate details
                    entities.TBL_LOGIN.Add(newUData);

                    //Packages the data to a variable with the corresponding given informaton to be sent to TBL_USER_DETAILS specifically
                    var newUDetail = new TBL_USER_DETAILS
                    {
                        UID = newUData.ID,
                        FIRSTNAME = model.textbox_FIRST_NAME,
                        LASTNAME = model.textbox_LAST_NAME,
                        DATE_CREATED = DateTime.Now,
                        DATE_MODIFIED = DateTime.Now,
                        PIN = "Undefined",
                        TIMEOUT = -1
                    };
                    //Adds a new entry in the TBL login with the appropriate details
                    entities.TBL_USER_DETAILS.Add(newUDetail);
                    //Saves changes and finalizes the entry
                    entities.SaveChanges();
                }

                //Sends them directly to the login page after registering
                return RedirectToAction("Index", "Login");  // Temporary
            }

            //If model state is invalid, it would throw them back to Registration index with an error message
            ViewBag.ErrorMessage = "Invalid";
            return View();
        }
    }
}