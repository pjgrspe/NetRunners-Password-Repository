using PasswordRepository.Helpers;
using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class LoginController : Controller
    {
        // GET: Login
        public ActionResult Index()
        {
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
                    var eData = entities.TBL_LOGIN.Where(x => x.EMAIL.Equals(model.LOGINEMAILUSER) || x.USERNAME.Equals(model.LOGINEMAILUSER)).FirstOrDefault();
                    if (eData != null)
                    {
                        var decryptedString = Encrypter.DecryptString(eData.PASSWORD);
                        if (Convert.ToString(decryptedString) == model.LOGINPASSWORD)
                        {
                            return Redirect("/Content/Index");
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
    }
}