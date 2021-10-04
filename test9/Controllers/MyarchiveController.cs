using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using test9.Models;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Globalization;

namespace test9.Controllers
{

    public class MyarchiveController : Controller
    {
        WeatherContext db = new WeatherContext();
        private String Year;

        // GET: Myarchive

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {return RedirectToAction("Login", "Home");
                //ViewBag.Archive = null;
                
            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(String Year)
        {

            if (User.Identity.IsAuthenticated)
            {
                this.Year = Year;
               int DateHour = DateTime.Now.Hour;
                // var archive = db.Archives.First(e => e.Year == Year);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://api.openweathermap.org/data/2.5/weather?q=%20Saint%20Petersburg,ru&appid=c986959d1fd2ecb4376e73e676671d28&units=metric");
                //HttpContent content = response.Content;
                // string data = await content.ReadAsStringAsync();
                var stringResult = await response.Content.ReadAsStringAsync();
                var rawWeather = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);
               String Humidity = rawWeather.Main.Humidity;
               String Temp = rawWeather.Main.Temp;
                String Pressure = rawWeather.Main.Pressure;
                int humidity = Int32.Parse(Humidity);
                //без CultureInfo была ошибка из за преобразования разделителя в виде запятой
                double temp = Double.Parse(Temp, CultureInfo.InvariantCulture);
                double pressure = Double.Parse(Pressure, CultureInfo.InvariantCulture);
                 //var archive = db.Archives.FirstOrDefault(e => e.Year == Year);
                var archive = db.Archives.Where(t =>(temp<= (t.Temperature*1.05))&&(humidity <=(t.Humidity*1.05))&& (pressure <= (t.Pressure * 1.05))).FirstOrDefault() ;
                 ViewBag.Archive = archive;
                List<Archive> data = db.Archives.Where(t => 
                (temp <= (t.Temperature * 1.05)&&temp>= (t.Temperature * 0.95)
                && (humidity <= (t.Humidity * 1.05)&&humidity>=t.Humidity*0.95) 
                && (pressure <= (t.Pressure * 1.05)&&pressure>=t.Pressure*0.95)
                &&(DateHour<=t.Time+3)&&(DateHour>=t.Time-3)
                )).ToList();
                return View(archive);
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }

        }

    }
}