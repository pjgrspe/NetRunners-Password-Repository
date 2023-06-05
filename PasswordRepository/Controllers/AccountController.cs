using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class AccountController : Controller
    {
        // GET: Deactivated
        public ActionResult Deactivated()
        {
            return View();
        }

        public ActionResult Deleted()
        {
            return View();
        }
    }
}