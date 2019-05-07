using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diploma.Models;
using System.Data.Entity;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db = new ApplicationContext();

        public ActionResult Index()
        {
            return View();
        }
    }
}