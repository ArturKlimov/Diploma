using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diploma.Models;
using System.Data.Entity;
using PagedList.Mvc;
using PagedList;

namespace Diploma.Controllers
{
    [RoutePrefix("home")]
    public class HomeController : Controller
    {
        ApplicationContext db = new ApplicationContext();

        [Route("~/home")]
        public ActionResult Index()
        {
            return View();
        }
        
        [Route("/allnews/{category}/{page}")]
        public ActionResult AllNews(int? category, int? page)
        {
            //Количество объектов на страницу
            int pageSize = 8;

            //Количество страниц
            int pageNumber;

            if (page == null)
            {
                pageNumber = 1;
            }
            else
            {
                pageNumber = (int)page;
            }

            List<New> news;


            if (category == null)
            {
                news = db.News.Include(n => n.Category).ToList();
            }
            else
            {
                news = db.News.Include(n => n.Category).Where(n => n.Category.ID == category).ToList();

                var selectedCategory = db.Categories.FirstOrDefault(c => c.ID == category).ID;
                ViewBag.SelectedCategory = selectedCategory;
            }

            var categories = db.Categories.ToList();

            ViewBag.Categories = categories;
            return PartialView("AllNews", news.ToPagedList(pageNumber, pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}