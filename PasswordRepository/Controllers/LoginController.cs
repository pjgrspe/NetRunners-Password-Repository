using PasswordRepository.Helpers;
using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Xml.Linq;


namespace PasswordRepository.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        //Cache magic code, code from breaking when pressing back button
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "*")]
        // Runs the code on page load
        public ActionResult Index()
        {
            //Checks if there is session in place, redirects to dashboard index if there is one already
            if (Session["ID"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }
            //Otherwise load the Login index page
            return View();
            
        }

        [HttpPost]
        //Prevents hacking and tampering of data
        [ValidateAntiForgeryToken]
        //Main login function
        public ActionResult ValidateLogin(LoginModel model)
        {
            //checks if Model is valid
            if (ModelState.IsValid)
            {
                //Sets the entity object
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    //sets eData to select the entry that matches the email or username in the database
                    var eData = entities.TBL_LOGIN.Where(x => x.EMAIL.Equals(model.textbox_LOGINEMAILUSER) || x.USERNAME.Equals(model.textbox_LOGINEMAILUSER)).FirstOrDefault();
                    //proceeds if entry match was found
                    if (eData != null)
                    {
                        //decrypts the encrypted password string that matches the selected data's password
                        var decryptedString = Encrypter.DecryptString(eData.PASSWORD);
                        //Checks if converted password string matches the password on the textbox
                        if (Convert.ToString(decryptedString) == model.textbox_LOGINPASSWORD)
                        {
                            //Cookie code for sessions
                            FormsAuthentication.SetAuthCookie(eData.ID.ToString(), true);
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(eData.ID, (eData.EMAIL ?? eData.USERNAME), DateTime.Now, DateTime.Now.AddDays(1), true, eData.ID.ToString());
                            string eCookie = FormsAuthentication.Encrypt(ticket);
                            HttpCookie httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, eCookie)
                            {
                                Path = FormsAuthentication.FormsCookiePath
                            };

                            //Sets the Session ID and Session username to the logged in user data
                            Session["ID"] = eData.ID;
                            Session["UserName"] = eData.USERNAME;

                            //sets eData to selected the entry that matches the current UID in TBL_USER_DETAILS
                            var dData = entities.TBL_USER_DETAILS.Where(x => x.UID.Equals(eData.ID)).FirstOrDefault();

                            //Sets the Session variables PIN and TO to the corresponding data on the table, also sets the timedout flag to false by default
                            Session["PIN"] = dData.PIN;
                            Session["TO"] = dData.TIMEOUT;
                            //TimedOut flag is used for the pin timer system
                            Session["timedout"] = false;
                            Session["Status"] = eData.STATUS;
                            Session["Access"] = eData.ACCESSLVL;
                            //Sets session timeout to three days
                            Session.Timeout = 1440;

                            //Cookie code
                            Response.Cookies.Add(httpCookie);
                            

                            if (eData.STATUS || eData.ACCESSLVL == false)
                            {
                                //Redirects to Index Dashboard after everything is settled
                                return RedirectToAction("Index", "Dashboard");
                            }
                            else if (eData.STATUS || eData.ACCESSLVL == true)
                            {
                                return RedirectToAction("Index", "AdminInterface");
                            }
                            else
                            {
                                return RedirectToAction("Deactivated", "Account");
                            }
                        }
                        else //Viewbag error for when the password do not match the username
                        {
                            ViewBag.ErrorMessage = "Password is incorrect!";
                            return View("Index");
                        }
                    }
                    //DOESNT WORK YET
                    else //Viewbag error for when the Username or Email does not exist in the database
                    {
                        ViewBag.ErrorMessage = "User doesn't exist!";
                        return View("Index");
                    }
                }
            }
            //Viewbag error for when the model is invalid
            ViewBag.ErrorMessage = "Invalid Model";
            return View("Index");
        }


        //LogOut Code
        public ActionResult LogOut()
        {
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
            return RedirectToAction("Index","Home");
        }

    }
}