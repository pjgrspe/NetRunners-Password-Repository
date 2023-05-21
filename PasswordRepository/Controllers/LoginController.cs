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

        public ActionResult Index()
        {
            if (Session["ID"] != null)
            {
                return RedirectToAction("Index", "Dashboard");
            }


            //using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
            //{
            //    var startDate = DateTime.Now;
            //    var eData = entities.TBL_PASSWORD_REPO.Where(x => x.EXPIRY_DATE >= startDate).ToList();

            //    foreach (var PassEntry in eData)
            //    {
            //        PassEntry.isActive = false;
            //        entities.SaveChanges();
            //    }
                


                return View();
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]

        public ActionResult Index(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                using (PassRepoDatabaseEntities entities = new PassRepoDatabaseEntities())
                {
                    var eData = entities.TBL_LOGIN.Where(x => x.EMAIL.Equals(model.textbox_LOGINEMAILUSER) || x.USERNAME.Equals(model.textbox_LOGINEMAILUSER)).FirstOrDefault();
                    if (eData != null)
                    {
                        var decryptedString = Encrypter.DecryptString(eData.PASSWORD);
                        if (Convert.ToString(decryptedString) == model.textbox_LOGINPASSWORD)
                        {
                            FormsAuthentication.SetAuthCookie(eData.ID.ToString(), true);
                            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(eData.ID, (eData.EMAIL ?? eData.USERNAME), DateTime.Now, DateTime.Now.AddDays(1), true, eData.ID.ToString());
                            string eCookie = FormsAuthentication.Encrypt(ticket);
                            HttpCookie httpCookie = new HttpCookie(FormsAuthentication.FormsCookieName, eCookie)
                            {
                                Path = FormsAuthentication.FormsCookiePath
                            };


                            Session["ID"] = eData.ID;
                            Session["UserName"] = eData.USERNAME;

                            var dData = entities.TBL_USER_DETAILS.Where(x => x.UID.Equals(eData.ID)).FirstOrDefault();

                            Session["PIN"] = dData.PIN;
                            Session["TO"] = dData.TIMEOUT;
                            Session.Timeout = 1440;

                            Response.Cookies.Add(httpCookie);
                            return RedirectToAction("Index", "Dashboard");  //For Prod
                            //return RedirectToAction("Test", "Dashboard");  //For Test
                            //return RedirectToAction("FormTest", "Dashboard"); //form test
                        }
                        else
                        {
                            ViewBag.ErrorMessage = "Password is incorrect!";
                            return View();
                        }
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Username or Email is incorrect!";
                        return View();
                    }
                }
            }
            return View();
        }

        public ActionResult LogOut()
        {
            Session["ID"] = null;
            Session.Abandon();
            FormsAuthentication.SignOut();
            return RedirectToAction("Index","Home");
        }
    }
}