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
            var notifications = db.Notifications.ToList();

            List<Notification> lastNotifications = new List<Notification>();

            int numberOfNotifications = notifications.Count;

            for (int i = 1; i <= 3; i++)
            {
                if (numberOfNotifications - i >= 0)
                {
                    lastNotifications.Add(notifications[numberOfNotifications - i]);
                }
                else
                {
                    break;
                }
            }
            return View(lastNotifications);
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
                news = db.News.Include(n => n.Category).OrderByDescending(i => i.ID).ToList();
            }
            else
            {
                news = db.News.Include(n => n.Category).Where(n => n.Category.ID == category).OrderByDescending(i => i.ID).ToList();

                var selectedCategory = db.Categories.FirstOrDefault(c => c.ID == category).ID;
                ViewBag.SelectedCategory = selectedCategory;
            }

            var categories = db.Categories.ToList();

            ViewBag.Categories = categories;
            return PartialView("AllNews", news.ToPagedList(pageNumber, pageSize));
        }

        [HttpGet]
        public ActionResult TestVideo()
        {
            return View("TestVideo");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}