using Diploma.Models;
using Diploma.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
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
        [Authorize]
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

            if (TempData["Success"] != null)
            {
                ViewBag.Success = TempData["Success"].ToString();
            }
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

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByEmailAsync(model.Email);

                if  (user == null)
                {
                    return RedirectToAction("/login");
                }
                else
                {
                    var provider = new DpapiDataProtectionProvider("SampleAppName");
                    UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("SampleTokenName"));

                    string hashCode = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                    var resetUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = hashCode }, protocol: Request.Url.Scheme);
                    
                    //настройка имэйла отправителя и получателя
                    MailAddress fromAddress = new MailAddress("noreplyfti123@gmail.com");
                    string fromPassword = "ftidrop1234";
                    
                    //настройка smtp клиента
                    SmtpClient smtp = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                    };

                    MailAddress toAddress = new MailAddress(model.Email);

                    MailMessage message = new MailMessage(fromAddress, toAddress)
                    {
                        Subject = "Восстановление пароля",
                        Body = "Для восстановления пароля перейдите по ссылке и измените свой пароль <a href=\"" + resetUrl + "\">Смена пароля</a>"
                    };

                    message.IsBodyHtml = true;

                    await smtp.SendMailAsync(message);

                    return RedirectToAction("/login");
                }
            }

            return RedirectToAction("login");
        }

        [HttpGet]
        public ActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                return RedirectToAction("/login");
            }
            else
            {
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return RedirectToAction("/login");
            }
            else
            {
                var provider = new DpapiDataProtectionProvider("SampleAppName");
                UserManager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(provider.Create("SampleTokenName"));

                var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);

                if (result.Succeeded)
                {
                    TempData["Success"] = "Пароль успешно изменен!";
                    return RedirectToAction("/login");
                }

                return View(model);
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}