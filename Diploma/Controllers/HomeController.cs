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

        [Route("/allnews/{page}")]
        public ActionResult AllNews(int? page)
        {
            //Количество объектов на страницу
            int pageSize = 1;

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

            //Все новости
            var news = db.News.Include(n => n.Category).ToList();

            return View(news.ToPagedList(pageNumber, pageSize));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}