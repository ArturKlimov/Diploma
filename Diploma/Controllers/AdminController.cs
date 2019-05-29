using Diploma.Models;
using Diploma.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;
using Microsoft.AspNet.Identity;
using System.Web.Helpers;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;

namespace Diploma.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        //Объявляем контекст данных
        ApplicationContext db = new ApplicationContext();
        
        //GET-запрос админ панель
        [HttpGet]
        public ActionResult Index(int? UserId)
        {

            ViewBag.Name = User.Identity.GetUserName();

            return View();
        }

        //GET-запрос на вывод списка новостей
        [HttpGet]
        public ActionResult GetNewsList()
        {
            var news = db.News.Include(n => n.Category).OrderByDescending(i => i.ID).Take(5).ToList();

            return PartialView(news);
        }

        [HttpGet]
        //GET-запрос на добавление новости
        public ActionResult AddNew()
        {

            SelectList categories = new SelectList(db.Categories, "ID", "Title");
            ViewBag.Categories = categories;

            return PartialView();
        }


        //POST-запрос создание новости
        [HttpPost]
        public ActionResult AddNew(New aNew)
        {
            if (ModelState.IsValid)
            {
                //Заполняем поле даты  
                aNew.Date = DateTime.Now;

                var image = WebImage.GetImageFromRequest();

                if (image != null)
                {
                    string extension = image.ImageFormat;
                    string fileName = DateTime.Now.ToString("ddMMyyyyhhmmss");
                    string filePath = "/Content/Images/" + fileName + "." + extension;

                    image.FileName = fileName;
                    image.Resize(800, 600);

                    image.Save(Server.MapPath(@"~" + filePath));

                    //Заполняем поле путь к изображению
                    aNew.ImagePath = filePath;
                }
                else
                {
                    aNew.ImagePath = "/Content/Images/default.jpg";
                }

                //Сохраняем новость в базе данных
                db.News.Add(aNew);
                db.SaveChanges();

            }
            return Redirect("/admin");
        }


        //GET-запрос на удаление новости
        [HttpGet]
        public ActionResult DeleteNew(int? id)
        {
            //если есть id
            if (id != null)
            {
                //Найти новость по ID новости
                var deleteNew = db.News.FirstOrDefault(n => n.ID == id);

                //если нашли новость
                if (deleteNew != null)
                {
                    if (deleteNew.ImagePath != null && deleteNew.ImagePath != "/Content/Images/default.jpg")
                    {
                        string fullPath = Request.MapPath("~" + deleteNew.ImagePath);

                        if (System.IO.File.Exists(fullPath))
                        {
                            System.IO.File.Delete(fullPath);
                        }
                    }

                    db.News.Remove(deleteNew);
                    db.SaveChanges();

                    return Redirect("/admin");
                }
                //если не нашли новость
                else
                {
                    return HttpNotFound();
                }
            }
            //если нет id
            else
            {
                return HttpNotFound();
            }
        }


        //GET-запрос на редактирование новости
        [HttpGet]
        public ActionResult EditNew(int? id)
        {
            //Если id передан
            if (id != null)
            {
                //Найти новость по ID
                var editNew = db.News.FirstOrDefault(n => n.ID == id);

                //Если новость найдена
                if (editNew != null)
                {
                    //Возвращаем новость во View
                    SelectList categories = new SelectList(db.Categories, "ID", "Title");
                    ViewBag.Categories = categories;
                    return View(editNew);
                }
                //Если новость не найдена
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return HttpNotFound();
            }
        }


        //POST-запрос на изменение новости
        [HttpPost]
        public ActionResult EditNew(New editNew)
        {
            if (ModelState.IsValid)
            {
                //Редактирование новости
                db.Entry(editNew).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Redirect("/admin");
        }

        //GET-запрос на получение списка новостей
        [HttpGet]
        public ActionResult GetEmailsList()
        {
            var emails = db.Emails.Include(e => e.Recipient).OrderByDescending(i => i.ID).Take(5).ToList();

            return PartialView(emails);
        }

        //GET-запрос на добавление почты в базу
        [HttpGet]
        public ActionResult AddEmail()
        {
            SelectList recipients = new SelectList(db.Recipients, "ID", "Title");
            ViewBag.Recipients = recipients;
            return PartialView();
        }


        //POST-запрос на добавление почты в базу
        [HttpPost]
        public ActionResult AddEmail(Email email)
        {
            if (ModelState.IsValid)
            {
                db.Emails.Add(email);
                db.SaveChanges();
            }
            return Redirect("/admin");
        }

        //GET-запрос на получения списка пользователей
        [HttpGet]
        public ActionResult GetUsersList()
        {
            var users = db.Users.OrderByDescending(u => u.Id).Take(5).ToList();

            return PartialView(users);
        }

        //GET-запрос на удаление почты пользователя
        [HttpGet]
        public ActionResult DeleteEmail(int? id)
        {
            var deleteEmail = db.Emails.FirstOrDefault(e => e.ID == id);

            if (id != null)
            {
                //если нашли почту
                if (deleteEmail != null)
                {
                    db.Emails.Remove(deleteEmail);
                    db.SaveChanges();
                    return Redirect("/admin");
                }
                //если не нашли почту
                else
                {
                    return HttpNotFound();
                }
            }
            //если нет id
            else
            {
                return HttpNotFound();
            }
        }

        //GET-запрос на добавление объявления
        [HttpGet]
        public ActionResult AddNotification()
        {
            var recipients = db.Recipients.ToList();
            ViewBag.Recipients = recipients;

            return PartialView();
        }

        //POST-запрос на добавление объявления
        [HttpPost]
        public async Task<ActionResult> AddNotification(Notification notification, int[] recipients, HttpPostedFileBase document1, HttpPostedFileBase document2)
        {
            if (ModelState.IsValid)
            {
                //Заполняем поле даты  
                notification.Date = DateTime.Now;

                //Сохраняем объявление в базе данных
                db.Notifications.Add(notification);
                db.SaveChanges();

                //Если пользователь выбрал какую-либо группу получателей
                if (recipients != null)
                {
                    //настройка имэйла отправителя и получателя
                    MailAddress fromAddress = new MailAddress("noreplyfti123@gmail.com");
                    string fromPassword = "ftidrop1234";

                    //Записываем все элементы почты в список emails
                    var emails = db.Emails.ToList();
                    
                    //Если в базе есть почты
                    if (emails.Count != 0)
                    {  
                        //если RecipientID у элемента почты совпадает с каким либо элементом в списке recipients, то берем его 
                        emails = emails.Where(e => recipients.Any(r => r == e.RecipientID)).ToList();

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
                                Subject = notification.Title,
                                Body = notification.Description
                            };

                            for (int i = 1; i < emails.Count(); i++)
                            {
                                message.Bcc.Add(emails[i].Mail);
                            }

                            if (document1 != null)
                            {
                                message.Attachments.Add(new Attachment(document1.InputStream, Path.GetFileName(document1.FileName)));
                            }

                            if (document2 != null)
                            {
                                message.Attachments.Add(new Attachment(document2.InputStream, Path.GetFileName(document2.FileName)));
                            }

                            await smtp.SendMailAsync(message);
                        }
                    }
                }

            }
            return Redirect("/admin");
        }

        //GET-запрос на вывод 5 последних объявлений
        [HttpGet]
        public ActionResult GetNotificationsList()
        {
            var notifications = db.Notifications.OrderByDescending(i => i.ID).Take(5).ToList();

            return PartialView(notifications);
        }

        //GET-запрос на редактирование объявления
        [HttpGet]
        public ActionResult EditNotification(int? id)
        {
            //Если id передан
            if (id != null)
            {
                //Найти объявление по ID и записать в переменную
                var editNotification = db.Notifications.FirstOrDefault(n => n.ID == id);

                //Если новость найдена
                if (editNotification != null)
                {
                    return View(editNotification);
                }
                //Если новость не найдена
                else
                {
                    return HttpNotFound();
                }
            }
            else
            {
                return HttpNotFound();
            }
        }

        //POST-запрос на редактирование объявления
        [HttpPost]
        public ActionResult EditNotification(Notification editNotification)
        {
            if (ModelState.IsValid)
            {
                //Редактирование новости
                db.Entry(editNotification).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect("/admin");
        }

        //GET-запрос на удаление объявления
        [HttpGet]
        public ActionResult DeleteNotification(int? id)
        {
            //если есть id
            if (id != null)
            {
                //Найти новость по ID новости
                var deleteNotification = db.Notifications.FirstOrDefault(n => n.ID == id);

                //если нашли новость
                if (deleteNotification != null)
                {
                    db.Notifications.Remove(deleteNotification);
                    db.SaveChanges();

                    return Redirect("/admin");
                }
                //если не нашли новость
                else
                {
                    return HttpNotFound();
                }
            }
            //если нет id
            else
            {
                return HttpNotFound();
            }
        }

        //GET-запрос на получение всех новостей
        [HttpGet]
        public ActionResult GetAllNotifications(string search)
        {
            List<Notification> notifications;
            if (search == null)
            {
                notifications = db.Notifications.ToList();
                
            }
            else
            {
                notifications = db.Notifications.Where(n => n.Title.Contains(search)).ToList();
            }
            return PartialView("GetAllNotifications", notifications);
        }

        //Запрос на получение предсатвления страницы всех пользователей
        [HttpGet]
        public ActionResult GetAllNotificationsList()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}