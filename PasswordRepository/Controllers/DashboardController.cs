﻿using PasswordRepository.Helpers;
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
            return View();
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

                var studentModel = new PasswordEntryModel()
                {
                    Passwords = data,
                    Password = new TBL_PASSWORD_REPO()
                };

                return View(studentModel);
            }
        }

        public ActionResult FormTest() // FOR DEBUGGING
        {
            return View();
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
                        UID = (int) Session["ID"],
                        PR_TITLE = model.textbox_PR_TITLE,
                        PR_EMAIL = model.textbox_PR_EMAIL,
                        PR_USERNAME = model.textbox_PR_USERNAME,
                        PR_PASSWORD = ePassword,
                        PR_URL = model.textbox_PR_URL,
                        PR_NOTES = model.textbox_PR_NOTES,
                        isTrashed = false,
                        isActive = false,
                    };
                    entities.TBL_PASSWORD_REPO.Add(newUData);
                    entities.SaveChanges();
                }
                return RedirectToAction("Test", "Dashboard");
                //return RedirectToAction("FormTest", "Dashboard");
            }

            ViewBag.ErrorMessage = "Invalid";
            return View();
        }
    }
}