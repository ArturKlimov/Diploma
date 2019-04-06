using Diploma.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Diploma.Controllers
{
    public class AdminController : Controller
    {
        //Объявляем контекст данных
        UniversityContext db = new UniversityContext();

        //GET-запрос админ панель
        public ActionResult Index()
        {
            var news = db.News;
            ViewBag.News = news;
            return View();
        }
        //GET-запрос создание новости
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
            return Redirect("/Admin/CreateNew");
        }
        
        //GET-запрос на удаление новости
        [HttpGet]
        public ActionResult DeleteNew()
        {

            return View();
        }
    }
}