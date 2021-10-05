using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using test9.Models;

namespace test9.Controllers
{
    public class HomeController : Controller
    {
        WeatherContext db = new WeatherContext();
        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.Extremum = null;
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public  ActionResult Index(string dayMonth)
        {
            if (string.IsNullOrEmpty(dayMonth))
            {
                return View();
            }

            // получаем из бд все объекты Extremum
            var extremum = db.Extrema.SingleOrDefault(e => e.Day_Month == dayMonth);
            // передаем все объекты в динамическое свойство Extremums в ViewBag
            ViewBag.Extremum = extremum;
            // возвращаем представление
            User user = null;
            using (WeatherContext db = new WeatherContext())
            {
                user = db.Users.FirstOrDefault(u => u.Login == User.Identity.Name);
            }
            if (user != null)
            {
                var userId = db.Users.Where(u => u.Login == User.Identity.Name).Select(u => u.Id).SingleOrDefault();
                var extremumId = extremum.Id;
                db.Requests.Add(new Request { UserId = userId, ForecastId = 0, ExtremumId = extremumId });
                db.SaveChanges();
            }

            return View();
        }
        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                // поиск пользователя в бд
                User user = null;
                using (WeatherContext db = new WeatherContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Login == model.Name && u.Password == model.Password);

                }
                if (user != null)
                {
                    FormsAuthentication.SetAuthCookie(model.Name, true);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Пользователя с таким логином и паролем нет");
                }
            }

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserRegister model)
        {
            if (ModelState.IsValid)
            {
                User user = null;
                using (WeatherContext db = new WeatherContext())
                {
                    user = db.Users.FirstOrDefault(u => u.Login == model.Name);
                }
                if (user == null)
                {
                    // создаем нового пользователя
                    using (WeatherContext db = new WeatherContext())
                    {
                        db.Users.Add(new User { Login = model.Name, Password = model.Password });
                        db.SaveChanges();

                        user = db.Users.Where(u => u.Login == model.Name && u.Password == model.Password).FirstOrDefault();
                    }
                    // если пользователь удачно добавлен в бд
                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.Name, true);
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь с таким логином уже существует");
                }
            }

            return View(model);
        }
        [AllowAnonymous]
        public ActionResult Logoff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }
    }
}