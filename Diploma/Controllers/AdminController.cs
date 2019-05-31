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
using Google.Apis.Services;
using Google.Apis.YouTube.v3.Data;
using Google.Apis.YouTube.v3;
using Diploma.ViewModels;

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
                    image.Resize(1280, 720);

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
            var users = db.Users.OrderByDescending(u => u.Id).ToList();

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

        //GET-запрос на получение всех объявлений по результатам поиска
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

        //GET-запрос на получение представления со всеми новостями и поиском по новостям
        [HttpGet]
        public ActionResult SearchNews()
        {
            return View();
        }

        //GET-запрос на получение всех новостей
        [HttpGet]
        public ActionResult GetSearchNews(string search)
        {
            List<New> news;
            if (search == null)
            {
                news = db.News.Include(n => n.Category).ToList();

            }
            else
            {
                news = db.News.Include(n => n.Category).Where(n => n.Title.Contains(search)).ToList();
            }
            return PartialView("GetSearchNews", news);
        }

        //GET-запрос на добавление видео
        [HttpGet]
        public ActionResult AddVideo()
        {
            return PartialView();
        }

        //POST-запрос на добавление видео
        [HttpPost]
        public ActionResult AddVideo(AddVideoViewModel model)
        {
            if (ModelState.IsValid)
            {
                var isUri = Uri.IsWellFormedUriString(model.VideoURL, UriKind.Absolute);

                if (isUri == true)
                {
                    var uri = new Uri(model.VideoURL);

                    if (uri.Host == "www.youtube.com")
                    {
                        var query = HttpUtility.ParseQueryString(uri.Query);

                        string videoId;

                        if (query.AllKeys.Contains("v"))
                        {
                            videoId = query["v"];
                        }
                        else
                        {
                            videoId = uri.Segments.Last();
                        }

                        var youtubeService = new YouTubeService(new BaseClientService.Initializer()
                        {
                            ApiKey = "AIzaSyCFClrn_jMQ7-6dDacoXtf4GdeWwwjG0_g",
                            ApplicationName = GetType().ToString()
                        });

                        var request = youtubeService.Videos.List("snippet,contentDetails,statistics");

                        request.Id = videoId;

                        var response = request.Execute();

                        var video = new Models.Video();

                        foreach (var v in response.Items)
                        {
                            video.YoutubeID = v.Id;
                            video.Title = v.Snippet.Title;
                            video.Description = v.Snippet.Description;
                            video.Date = DateTime.Now;
                        }

                        db.Videos.Add(video);
                        db.SaveChanges();
                    }
                }
            }
            return Redirect("/admin");
        }

        //GET-запрос на добавление мероприятия
        [HttpGet]
        public ActionResult AddEvent()
        {
            return PartialView();
        }

        //POST-запрос на добавление мероприятия
        [HttpPost]
        public ActionResult AddEvent(Event aEvent, string startTime, string endTime)
        {
            if (ModelState.IsValid)
            {
                if(aEvent.StartDate == null)
                {
                    aEvent.StartDate = DateTime.Now.Date;
                }

                if (aEvent.EndDate == null)
                {
                    aEvent.EndDate = DateTime.Now.Date;
                }

                //Если поле со времнем было заполнено
                if (startTime != "")
                {
                    //То из строки времени вырезаем первые два символа, конвертируем их в число и записываем как часы к дате
                    aEvent.StartDate = aEvent.StartDate.AddHours(Convert.ToDouble(startTime.Substring(0, 2)));
                    aEvent.StartDate = aEvent.StartDate.AddMinutes(Convert.ToDouble(startTime.Substring(3, 2)));
                }
                else
                {
                    aEvent.StartDate = aEvent.StartDate.AddHours(12);
                }
                
                //
                if (endTime != "")
                {
                    aEvent.EndDate = aEvent.EndDate.AddHours(Convert.ToDouble(endTime.Substring(0, 2)));
                    aEvent.EndDate = aEvent.EndDate.AddMinutes(Convert.ToDouble(endTime.Substring(3, 2)));
                }
                else
                {
                    aEvent.EndDate = aEvent.EndDate.AddHours(12);
                }

                db.Events.Add(aEvent);
                db.SaveChanges();
            }

            return Redirect("/admin");
        }

        //GET-запрос на получение последних 5 мероприятий для админ-панели
        [HttpGet]
        public ActionResult Get5Events()
        {
            var events = db.Events.OrderByDescending(i => i.ID).Take(5).ToList();

            return PartialView(events);
        }

        //GET-запрос на редактирование объявления
        [HttpGet]
        public ActionResult EditEvent(int? id)
        {
            //Если id передан
            if (id != null)
            {
                //Найти объявление по ID и записать в переменную
                var editEvent = db.Events.FirstOrDefault(n => n.ID == id);

                //Если новость найдена
                if (editEvent != null)
                {
                    ViewBag.StartTime = editEvent.StartDate.ToString("t");
                    ViewBag.EndTime = editEvent.EndDate.ToString("t");

                    return View(editEvent);
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
        public ActionResult EditEvent(Event editEvent, string startTime, string endTime)
        {
            if (ModelState.IsValid)
            {
                if (editEvent.StartDate == null)
                {
                    editEvent.StartDate = DateTime.Now.Date;
                }

                if (editEvent.EndDate == null)
                {
                    editEvent.EndDate = DateTime.Now.Date;
                }

                //Если поле со времнем было заполнено
                if (startTime != "")
                {
                    //То из строки времени вырезаем первые два символа, конвертируем их в число и записываем как часы к дате
                    editEvent.StartDate = editEvent.StartDate.AddHours(Convert.ToDouble(startTime.Substring(0, 2)));
                    editEvent.StartDate = editEvent.StartDate.AddMinutes(Convert.ToDouble(startTime.Substring(3, 2)));
                }
                else
                {
                    editEvent.StartDate = editEvent.StartDate.AddHours(12);
                }

                //
                if (endTime != "")
                {
                    editEvent.EndDate = editEvent.EndDate.AddHours(Convert.ToDouble(endTime.Substring(0, 2)));
                    editEvent.EndDate = editEvent.EndDate.AddMinutes(Convert.ToDouble(endTime.Substring(3, 2)));
                }
                else
                {
                    editEvent.EndDate = editEvent.EndDate.AddHours(12);
                }

                //Редактирование мероприятия
                db.Entry(editEvent).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Redirect("/admin");
        }

        //GET-запрос на удаление объявления
        [HttpGet]
        public ActionResult DeleteEvent(int? id)
        {
            //если есть id
            if (id != null)
            {
                //Найти новость по ID новости
                var deleteEvent = db.Events.FirstOrDefault(n => n.ID == id);

                //если нашли новость
                if (deleteEvent != null)
                {
                    db.Events.Remove(deleteEvent);
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

        //GET-запрос на получение представления со всеми мероприятиями и поиском по мероприятиям
        [HttpGet]
        public ActionResult SearchEvents()
        {
            return View();
        }

        //GET-запрос на получение всех мероприятий
        [HttpGet]
        public ActionResult GetSearchEvents(string search)
        {
            List<Event> events;
            if (search == null)
            {
                events = db.Events.ToList();

            }
            else
            {
                events = db.Events.Where(n => n.Title.Contains(search)).ToList();
            }
            return PartialView("GetSearchEvents", events);
        }

        //GET-запрос на получение последних 5 видео
        [HttpGet]
        public ActionResult Get5Videos()
        {
            var videos = db.Videos.OrderByDescending(i => i.ID).Take(5).ToList();

            return PartialView(videos);
        }

        //GET-запрос на удаление видео
        [HttpGet]
        public ActionResult DeleteVideo(int? id)
        {
            //если есть id
            if (id != null)
            {
                //Найти новость по ID новости
                var deleteVideo = db.Videos.FirstOrDefault(n => n.ID == id);

                //если нашли новость
                if (deleteVideo != null)
                {
                    db.Videos.Remove(deleteVideo);
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

        //GET-запрос на получение представления со всеми мероприятиями и поиском по мероприятиям
        [HttpGet]
        public ActionResult SearchVideos()
        {
            return View();
        }

        //GET-запрос на получение всех видео
        [HttpGet]
        public ActionResult GetSearchVideos(string search)
        {
            List<Models.Video> videos;
            if (search == null)
            {
                videos = db.Videos.ToList();

            }
            else
            {
                videos = db.Videos.Where(n => n.Title.Contains(search)).ToList();
            }
            return PartialView("GetSearchVideos", videos);
        }

        //GET-запрос на получение представления со всеми мероприятиями и поиском по мероприятиям
        [HttpGet]
        public ActionResult SearchEmails()
        {
            return View();
        }

        //GET-запрос на получение всех видео
        [HttpGet]
        public ActionResult GetSearchEmails(string search)
        {
            List<Email> emails;
            if (search == null)
            {
                emails = db.Emails.ToList();

            }
            else
            {
                emails = db.Emails.Where(n => n.Name.Contains(search) || n.Mail.Contains(search)).ToList();
            }
            return PartialView("GetSearchEmails", emails);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}