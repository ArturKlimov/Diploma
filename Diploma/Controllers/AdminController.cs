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

namespace Diploma.Controllers
{
    [Authorize]
    [RoutePrefix("admin")]
    public class AdminController : Controller
    {
        //Объявляем контекст данных
        ApplicationContext db = new ApplicationContext();
        //GET-запрос админ панель
        [Route("~/admin")]
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
            var news = db.News.Include(n => n.Category).ToList();

            List<New> lastNews = new List<New>();

            int numberOfNews = news.Count;

            for (int i = 1; i <= 5; i++)
            {
                if (numberOfNews - i >= 0)
                {
                    lastNews.Add(news[numberOfNews - i]);
                }
                else
                {
                    break;
                }
            }

            return PartialView(lastNews);
        }

        [HttpGet]
        //GET-запрос на добавление новости
        public ActionResult AddNew()
        {

            SelectList categories = new SelectList(db.Categories, "ID", "Title");
            ViewBag.Categories = categories;

            var recipients = db.Recipients.ToList();
            ViewBag.Recipients = recipients;

            return PartialView();
        }


        //POST-запрос создание новости
        [HttpPost]
        public ActionResult AddNew(New aNew, HttpPostedFileBase imageFile, int[] recipients)
        {
            if (ModelState.IsValid)
            {
                //Заполняем поле даты  
                aNew.Date = DateTime.Now;

                if (imageFile != null)
                {
                    //Работа с изображением
                    string extension = Path.GetExtension(imageFile.FileName);
                    string fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + extension;
                    string filePath = "/Content/Images/" + fileName;
                    fileName = Path.Combine(Server.MapPath(@"~/Content/Images/"), fileName);
                    imageFile.SaveAs(fileName);

                    //Заполняем поле путь к изображению
                    aNew.ImagePath = filePath;
                }
                else
                {
                    aNew.ImagePath = "";
                }

                //Сохраняем новость в базе данных
                db.News.Add(aNew);
                db.SaveChanges();

                //Для получения данных в EmailController
                if (recipients != null)
                {
                    //Передаем заголовок и описание новости
                    TempData["subject"] = aNew.Title;
                    TempData["body"] = aNew.Description;

                    //Передаем список получателей
                    TempData["recipients"] = recipients.ToList();

                    //Переходим на метод отправки
                    return RedirectToAction("Index", "Email");
                }

            }
            return Redirect("/admin");
        }


        //GET-запрос на удаление новости
        [Route("/deletenew/{id}")]
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
        [Route("/editnew/{id}")]
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
            //Редактирование новости
            db.Entry(editNew).State = EntityState.Modified;
            db.SaveChanges();

            return Redirect("/admin");
        }

        //GET-запрос на получение списка новостей
        [HttpGet]
        public ActionResult GetEmailsList()
        {
            var emails = db.Emails.Include(e => e.Recipient).ToList();

            List<Email> lastEmails = new List<Email>();

            int numberOfEmails = emails.Count;

            for (int i = 1; i <= 5; i++)
            {
                if (numberOfEmails - i >= 0)
                {
                    lastEmails.Add(emails[numberOfEmails - i]);
                }
                else
                {
                    break;
                }
            }
            return PartialView(lastEmails);
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
            var users = db.Users.ToList();

            List<ApplicationUser> lastUsers = new List<ApplicationUser>();

            int numberOfUsers = users.Count;

            for (int i = 1; i <= 5; i++)
            {
                if (numberOfUsers - i >= 0)
                {
                    lastUsers.Add(users[numberOfUsers - i]);
                }
                else
                {
                    break;
                }
            }

            return PartialView(lastUsers);
        }

        //GET-запрос на удаление почты пользователя
        [HttpGet]
        [Route("/deleteemail/{id}")]
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

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}