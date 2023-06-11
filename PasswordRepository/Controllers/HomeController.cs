using PasswordRepository.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;

namespace PasswordRepository.Controllers
{
    public class HomeController : Controller
    {
        //Launches index homepage
        public ActionResult Index()
        {
            return View();
        }

        //Launches About homepage
        public ActionResult About()
        {
            return View();
        }


        //Launches Contact homepage
        public ActionResult Contact()
        {
            if (TempData["SentMessage"] != null)
            {
                ViewBag.SuccessMessage = TempData["SentMessage"].ToString();
            }
            if (TempData["FailedMessage"] != null)
            {
                ViewBag.FailedMessage = TempData["FailedMessage"].ToString();
            }
            return View();
        }

        [HttpPost]
        //Main login function
        public ActionResult SendEmail(ContactViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    TempData["SentMessage"] = "Email sent to the developers! Thanks for your feedback.";
                    MailMessage ContactUsEmail = new MailMessage();
                    ContactUsEmail.From = new MailAddress(model.Email);
                    //var NREmail = "";
                    ContactUsEmail.To.Add("netrunners.dev@gmail.com");
                    ContactUsEmail.Subject = model.Subject;
                    ContactUsEmail.Body = "Sent by: " + Convert.ToString(model.Email) + "\n" + "Name: " + Convert.ToString(model.Name) + "\n\nMessage:\n" +model.Message;


                    SmtpClient smtp = new SmtpClient();
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;


                    NetworkCredential nc = new NetworkCredential("netrunners.dev@gmail.com", "bsaabsawtmbllblc");
                    smtp.UseDefaultCredentials = true;
                    smtp.Credentials = nc;

                    //smtp.Credentials = new System.Net.NetworkCredential("netrunners.dev@gmail.com", "netrunner123");
                    smtp.Send(ContactUsEmail);
                    ModelState.Clear();
                    //ModelState.Clear();
                    TempData["SentMessage"] = "Email sent to the developers! Thanks for your feedback";
                    return RedirectToAction("Contact", "Home");
                }
                catch (Exception)
                {
                    //ModelState.Clear();
                    ModelState.Clear();
                    TempData["FailedMessage"] = "Sorry, we are facing problems in the connection.";
                    return RedirectToAction("Contact", "Home");
                }
            }
            return View("Index");
        }
    }
}