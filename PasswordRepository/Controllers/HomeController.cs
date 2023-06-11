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
            ViewBag.Message = "Your home page.";

            return View();
        }

        //Launches About homepage
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }


        //Launches Contact homepage
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        //Prevents hacking and tampering of data
        [ValidateAntiForgeryToken]
        //Main login function
        public ActionResult SendEmail(ContactViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
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
                    ViewBag.Message = "Email sent to the developers! Thanks for your feedback";
                }
                catch (Exception ex)
                {
                    //ModelState.Clear();
                    ModelState.Clear();
                    ViewBag.Message = $" Sorry we are facing Problem here {ex.Message}";
                }
            }
            return View("Contact");
        }
    }
}