using System.Web.Mvc;
using System.Net;
using System.Net.Mail;
using Diploma.Models;
using System.Linq;
using System.Collections.Generic;

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



            //Все почты
            var emails = db.Emails.ToList();

            var recipients = TempData["recipients"] as IList<int>;

            bool check = false;

            //Если в базе есть почты
            if (emails.Count() != 0)
            {
                //Пробегаем по всем почтам
                for (int i = 0; i < emails.Count(); i++)
                {
                    //Пробегаем по всем ВЫБРАННЫМ группам получателей
                    for (int j = 0; j < recipients.Count(); j++)
                    {
                        //Если ID группы получателя в почте равен выбранному ID из формы
                        if (emails[i].RecipientID == recipients[j])
                        {
                            check = true;
                        }
                    }

                    //Если ID группы получателей не совпал ни с чем из выбранных, тогда удаляем из списка эту почту
                    if (check == false)
                    {
                        emails.Remove(emails[i]);
                    }
                    //Если совпал, реверсируем переменную для проверки
                    else
                    {
                        check = false;
                    }
                }
            }

            //Если в списке остались почты
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