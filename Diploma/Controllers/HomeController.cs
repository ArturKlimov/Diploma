using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Diploma.Models;
using System.Linq;
using System.Data.Entity;

namespace Diploma.Controllers
{
    public class HomeController : Controller
    {
        UniversityContext db = new UniversityContext();

        public ActionResult Index()
        {
            return View();
        }
    }
}