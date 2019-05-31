using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diploma.Models;
using System.Data.Entity;
using PagedList.Mvc;
using PagedList;
using Diploma.ViewModels;

namespace Diploma.Controllers
{
    
    public class HomeController : Controller
    {
        ApplicationContext db = new ApplicationContext();

        public ActionResult Index()
        {
            var notifications = db.Notifications.OrderByDescending(i => i.ID).Take(3).ToList();

            var news = db.News.Include(n => n.Category).OrderByDescending(i => i.ID).Take(6).ToList();

            var videos = db.Videos.OrderByDescending(i => i.ID).Take(6).ToList();

            var events = db.Events.OrderByDescending(i => i.ID).Take(10).ToList();

            HomeViewModel viewModel = new HomeViewModel
            {
                Notifications = notifications,
                News = news,
                Videos = videos,
                Events = events
            };

            return View(viewModel);
        }
        
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

        public ActionResult GetAllNotifications()
        {
            return PartialView();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}