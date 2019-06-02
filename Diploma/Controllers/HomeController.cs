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

        //Для контроля символов во View
        static string CutTheString(int maxLength, string userString)
        {
            string cutString = userString;

            if (cutString.Length > maxLength)
            {
                for (var i = maxLength - 1; i < userString.Length; i++)
                {
                    if (userString[i] == ' ')
                    {
                        cutString = cutString.Substring(0, i + 1);
                        break;
                    }
                }
            }
            return cutString;
        }

        public ActionResult Index()
        {
            var notifications = db.Notifications.OrderByDescending(i => i.ID).Take(3).ToList();

            foreach (var n in notifications)
            {
                n.Title = CutTheString(30, n.Title);
                n.Description = CutTheString(104, n.Description);
            }

            var news = db.News.Include(n => n.Category).OrderByDescending(i => i.ID).Take(6).ToList();

            var videos = db.Videos.OrderByDescending(i => i.ID).Take(6).ToList();

            foreach (var n in news)
            {
                n.Title = CutTheString(30, n.Title);
            }

            foreach (var n in videos)
            {
                n.Title = CutTheString(43, n.Title);
            }


            var events = db.Events.OrderByDescending(i => i.ID).Take(10).ToList();

            foreach (var n in events)
            {
                n.Title = CutTheString(66, n.Title);
            }

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
                    var bread = CutTheString(60, thisNew.Title);
                    ViewBag.Bread = bread;
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

            foreach (var n in notifications)
            {
                n.Title = CutTheString(80, n.Title);
                n.Description = CutTheString(370, n.Description);
            }

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

            foreach(var e in events)
            {
                e.Title = CutTheString(80, e.Title);
                e.Description = CutTheString(370, e.Description);
            }

            return PartialView("GetAllEvents", events.ToPagedList(pageNumber, pageSize));
        }

        public ActionResult GetOneVideo(int? id)
        {
            if (id != null)
            {
                var thisVideo = db.Videos.FirstOrDefault(v => v.ID == id);

                if (thisVideo != null)
                {
                    var bread = CutTheString(60, thisVideo.Title);
                    ViewBag.Bread = bread;
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
                    var bread = CutTheString(60, thisNotification.Title);
                    ViewBag.Bread = bread;
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
                var bread = CutTheString(60, thisEvent.Title);
                ViewBag.Bread = bread;
                return View(thisEvent);
            }
            else
            {
                return HttpNotFound();
            }
        }

        public ActionResult GetTable()
        {
            return View();
        }

        [HttpGet]
        public ActionResult GetSearch(string search)
        {
            if (search != "")
            {
                var news = db.News.Where(n => n.Title.Contains(search)).ToList();
                foreach(var n in news)
                {
                    n.Title = CutTheString(30, n.Title);
                }

                var notifications = db.Notifications.Where(n => n.Title.Contains(search)).ToList();
                foreach (var n in notifications)
                {
                    n.Title = CutTheString(30, n.Title);
                }

                var events = db.Events.Where(n => n.Title.Contains(search)).ToList();
                foreach (var n in events)
                {
                    n.Title = CutTheString(30, n.Title);
                }

                var videos = db.Videos.Where(n => n.Title.Contains(search)).ToList();
                foreach (var n in videos)
                {
                    n.Title = CutTheString(30, n.Title);
                }

                HomeViewModel model = new HomeViewModel
                {
                    News = news,
                    Notifications = notifications,
                    Events = events,
                    Videos = videos
                };
                return View(model);
            }
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}