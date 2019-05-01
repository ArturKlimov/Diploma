using Diploma.Models;
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
        UniversityContext db = new UniversityContext();

        //GET-запрос админ панель
        [Route("~/admin")]
        public ActionResult Index(int? UserId)
        {
            var news = db.News.Include(n => n.Category);

            ViewBag.Name = User.Identity.GetUserName();
            return View(news);
        }
        
        
        //GET-запрос создание новости
        [Route("/createnew")]
        [HttpGet]
        public ActionResult CreateNew()
        {
            SelectList categories = new SelectList(db.Categories, "ID", "Title");
            ViewBag.Categories = categories; 
            return View();
        }

        
        //POST-запрос создание новости
        [HttpPost]
        public ActionResult CreateNew(New aNew, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                //Заполняем поле даты  
                aNew.Date = DateTime.Now;

                //Работа с изображением
                string extension = Path.GetExtension(imageFile.FileName);
                string fileName = DateTime.Now.ToString("ddMMyyyyhhmmss") + extension;
                string filePath = "/Content/Images/" + fileName;
                fileName = Path.Combine(Server.MapPath(@"~/Content/Images/"), fileName);
                imageFile.SaveAs(fileName);

                //Заполняем поле путь к изображению
                aNew.ImagePath = filePath;
                
                //Сохраняем новость в базе данных
                db.News.Add(aNew);
                db.SaveChanges();
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

                    return RedirectToAction("/admin");
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


        //POST-запрос на удаление новости
        [HttpPost]
        public ActionResult EditNew(New editNew)
        {
            //Редактирование новости
            db.Entry(editNew).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("/admin");
        }
    }
}