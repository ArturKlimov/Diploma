using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net;
using System.Net.Mail;

namespace Diploma.Controllers
{
    public class EmailController : Controller
    {
        public ActionResult Index()
        {
            //настройка имэйла отправителя и получателя
            MailAddress fromAddress = new MailAddress("noreplyfti123@gmail.com");
            string fromPassword = "ftidrop1234";
            MailAddress toAddress = new MailAddress("dniwevenuziwe1@mail.ru");

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


            MailMessage message = new MailMessage(fromAddress, toAddress)
            {
                Subject = TempData["subject"].ToString(),
                Body = TempData["body"].ToString()
            };
            smtp.Send(message);
            return Redirect("/admin");
        }
    }
}