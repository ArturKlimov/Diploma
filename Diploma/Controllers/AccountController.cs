using Diploma.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Diploma.Controllers
{
    [RoutePrefix("account")]
    public class AccountController : Controller
    {
        //Получение UserManager
        private ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        //Объявляем контекст данных
        ApplicationContext db = new ApplicationContext();

        //Вывод представления для регистрации
        [Authorize]
        [Route("/register")]
        public ActionResult Register()
        {
            return View();
        }

        //POST-запрос на регистрацию пользователя
        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            //Если все данные в форме для регистрации введены правильно
            if (ModelState.IsValid)
            {
                //Инициализируем нового пользователя, учитывая данные из формы
                ApplicationUser user = new ApplicationUser { UserName = model.Login, Email = model.Email, Name = model.Name };
                
                //Создаем пользователя и записываем результат создания в переменную result
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);

                //Если создание пользователя прошло успешно
                if (result.Succeeded)
                {
                    return Redirect("/admin");
                }

                else
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }
            }
            return View(model);
        }

        //GET-запрос на удаление пользователя
        [Route("/account/deleteuser/{id}")]
        [Authorize]
        [HttpGet]
        public async Task<ActionResult> DeleteUser(string id)
        {
            //Ищем пользователя по имени
            ApplicationUser user = await UserManager.FindByIdAsync(id);

            //Находим количество пользователей
            var usersCount = db.Users.ToList().Count;

            //Если пользователь найден и количествоп пользователй не равно 1
            if (user != null && usersCount != 1)
            {
                IdentityResult result = await UserManager.DeleteAsync(user);
            }
            return Redirect("/admin");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        [Route("/login")]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await UserManager.FindAsync(model.Login, model.Password);
                if (user == null)
                {
                    ModelState.AddModelError("", "Неверный логин или пароль.");
                }
                else
                {
                    ClaimsIdentity claim = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
                    AuthenticationManager.SignOut();
                    AuthenticationManager.SignIn(new AuthenticationProperties { IsPersistent = true }, claim);

                    if (String.IsNullOrEmpty(returnUrl))
                        return RedirectToAction("../admin");
                    return Redirect(returnUrl);
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(model);
        }

        [Route("/logout")]
        public ActionResult Logout()
        {
            AuthenticationManager.SignOut();
            return RedirectToAction("/login");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}