using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using Diploma.Models;
using System.Linq;

namespace Diploma.Controllers
{
    public class EmailController : Controller
    {
        ApplicationContext db = new ApplicationContext();

        public ActionResult Index()
        {
            //настройка имэйла отправителя и получателя
            MailAddress fromAddress = new MailAddress("noreplyfti123@gmail.com");
            string fromPassword = "ftidrop1234";

            var emails = db.Emails.ToList();

            if (emails.Count != 0)
            {
                //настройка smtp клиента
                SmtpClient smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com",
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                MailAddress toAddress = new MailAddress(emails.First().Mail);

                MailMessage message = new MailMessage(fromAddress, toAddress)
                {
                    Subject = TempData["subject"].ToString(),
                    Body = TempData["body"].ToString()
                };

                for(int i = 1; i < emails.Count(); i++)
                {
                    message.Bcc.Add(emails[i].Mail);    
                }

                smtp.Send(message);
            }
            return Redirect("/admin");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}