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
using System.IO;

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

        public ActionResult GetOneNew(int? id)
        {

            if (id != null)
            {
                var thisNew = db.News.Include(c => c.Category).FirstOrDefault(n => n.ID == id);
                if (thisNew != null)
                {
                    var categories = db.Categories.ToList();
                    ViewBag.Categories = categories;
                    return View(thisNew);
                }
            }

            return HttpNotFound();
        }

        public ActionResult GetAllNotifications(int? page)
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

            var notifications = db.Notifications.OrderByDescending(i => i.ID);

            return PartialView("GetAllNotifications", notifications.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetAllVideos(int? page)
        {
            //Количество объектов на страницу
            int pageSize = 12;

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

            var videos = db.Videos.OrderByDescending(i => i.ID);

            return PartialView("GetAllVideos", videos.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetAllEvents(int? page)
        {
            //Количество объектов на страницу
            int pageSize = 4;

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

            var events = db.Events.OrderByDescending(i => i.ID);

            return PartialView("GetAllEvents", events.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetOneVideo(int? id)
        {
            if (id != null)
            {
                var thisVideo = db.Videos.FirstOrDefault(v => v.ID == id);

                if (thisVideo != null)
                {
                    return View(thisVideo);
                }
            }
            return HttpNotFound();
        }

        public ActionResult GetOneNotification(int? id)
        {
            if (id != null)
            {
                var thisNotification = db.Notifications.FirstOrDefault(n => n.ID == id);

                if (thisNotification != null)
                {
                    if (thisNotification.DocumentPath1 != "")
                    {
                        ViewBag.DocumentName1 = Path.GetFileName(thisNotification.DocumentPath1);
                    }
                    if (thisNotification.DocumentPath2 != "")
                    {
                        ViewBag.DocumentName2 = Path.GetFileName(thisNotification.DocumentPath2);
                    }

                    return View(thisNotification);
                }
            }
            return HttpNotFound();
        }

        public ActionResult GetOneEvent(int? id)
        {
            if (id != null)
            {
                var thisEvent = db.Events.FirstOrDefault(n => n.ID == id);

                return View(thisEvent);
            }
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