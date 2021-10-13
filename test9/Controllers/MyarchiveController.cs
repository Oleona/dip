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
using System.Text;

namespace test9.Controllers
{

    public class MyarchiveController : Controller
    {
        WeatherContext db = new WeatherContext();

        const double k1 = 1.05;//  верхний коэффициент для границ выборки из базы. Подобран опытным путем
        const double k2 = 0.95; // нижний
        double funkt;
        int funkh;
        double funkp;

        public List<ResultForecast> PartForecast(IEnumerable<Archive> f)
        {
            var resultForecast = new List<ResultForecast>();
            funkt = 0; funkh = 0; funkp = 0;
            foreach (Archive p in f)
            {
                if (p.Temperature.HasValue)
                {
                    funkt += p.Temperature.Value;
                }
                if (p.Humidity.HasValue)
                {
                    funkh += p.Humidity.Value;
                }

                if (p.Pressure.HasValue)
                {
                    funkp += p.Pressure.Value;
                }           
            }
            funkt = Math.Round(funkt / f.Count(), 2);
            funkh = funkh / f.Count();
            funkp = Math.Round(funkp / f.Count(), 2);
            ResultForecast resultFor = new ResultForecast(funkt, funkh, funkp);
            resultForecast.Add(resultFor);
            return resultForecast;
        }

        // GET: Myarchive

        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }

        [HttpPost]
        public async Task<ActionResult> Index(String buttonName)
        {
            if (User.Identity.IsAuthenticated)
            {
                int DateHour = DateTime.Now.Hour;

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync(
                    "https://api.openweathermap.org/data/2.5/weather?q=%20Saint%20Petersburg,ru&appid=c986959d1fd2ecb4376e73e676671d28&units=metric");
                var stringResult = await response.Content.ReadAsStringAsync();
                var rawWeather = JsonConvert.DeserializeObject<OpenWeatherResponse>(stringResult);

                int humidity = Int32.Parse(rawWeather.Main.Humidity);
                //без CultureInfo была ошибка из за преобразования разделителя в виде запятой
                double temp = Double.Parse(rawWeather.Main.Temp, CultureInfo.InvariantCulture);
                double pressure = Double.Parse(rawWeather.Main.Pressure, CultureInfo.InvariantCulture);
                
                //Выбираем из базы значения температуры, влажности, давления, отличающиеся от погоды на данный момент на 
                // подобранный коэффициент с учетом времени ( не более часа в обе стороны разброс). Из этой выборки находим даты,
                // в которые погода совпадала с текущей и выбираем даты на 1 больше- так как прогноз нужен на следующий день
                var dataDayMonthYear = db.Archives
                    .Where(t => temp <= t.Temperature * k1 && temp >= t.Temperature * k2)
                    .Where(t => humidity <= t.Humidity * k1 && humidity >= t.Humidity * k2)
                    .Where(t => pressure <= t.Pressure * k1 && pressure >= t.Pressure * k2)
                    .Where(t => t.Time >= DateHour - 1 && t.Time <= DateHour + 1 ||
                                DateHour + 1 == 24 && t.Time == 0)
                    //.ToList()
                    .Select(t => new DayMonthYearResult { DayResult = t.Day.Value + 1, MonthResult = t.Month.Value, YearResult = t.Year.Value })
                    .ToList();

                var forecastForNightPrediction = new List<Archive>();
                var forecastForMorningPrediction = new List<Archive>();
                var forecastForDayPrediction = new List<Archive>();
                var forecastForEveningPrediction = new List<Archive>();

                for (int i = 0; i < dataDayMonthYear.Count; i++)
                {
                    var myDayResult = dataDayMonthYear[i].DayResult;
                    var myMonthResult = dataDayMonthYear[i].MonthResult;
                    var myYearResult = dataDayMonthYear[i].YearResult;

                    // Выборка из базы это прогноз суточный для ночи, дня и т.д ( 4 периода) каждого из дней,
                    // которые подошли для прогноза. Потом для каждого объединяем в 4 общих-
                    //  на ночь всех суток, утро всех суток, день всех суток, вечер всех суток из выборки
                    db.Archives
                        .Where(v => v.Day == myDayResult && v.Month == myMonthResult && v.Year == myYearResult && (v.Time == 0 || v.Time == 3))
                        .ToList()
                        .ForEach(it => forecastForNightPrediction.Add(it));                   
                    db.Archives.Where(v => v.Day == myDayResult && v.Month == myMonthResult && v.Year == myYearResult && (v.Time == 6 || v.Time == 9))
                        .ToList().
                        ForEach(it => forecastForMorningPrediction.Add(it));
                    db.Archives.Where(v => v.Day == myDayResult && v.Month == myMonthResult && v.Year == myYearResult && (v.Time == 12 || v.Time == 15))
                        .ToList()
                        .ForEach(it => forecastForDayPrediction.Add(it));
                    db.Archives.Where(v => v.Day == myDayResult && v.Month == myMonthResult && v.Year == myYearResult && (v.Time == 18 || v.Time == 21))
                        .ToList()
                        .ForEach(it => forecastForEveningPrediction.Add(it));
                }

                var FullForecast = new List<ResultForecast>();
                // Находим средние значения, как наш прогноз для 4 частей суток, записываем в общий прогноз
                PartForecast(forecastForNightPrediction).ForEach(it => FullForecast.Add(it));
                PartForecast(forecastForMorningPrediction).ForEach(it => FullForecast.Add(it));
                PartForecast(forecastForDayPrediction).ForEach(it => FullForecast.Add(it));
                PartForecast(forecastForEveningPrediction).ForEach(it => FullForecast.Add(it));

                ViewBag.ResultFullForecast = FullForecast;
                
                StringBuilder writeForecastInBase = new StringBuilder();
                foreach (ResultForecast rf in FullForecast)
                {
                    writeForecastInBase.Append(rf.tempe.ToString() + " ");
                    writeForecastInBase.Append(rf.pres.ToString() + " ");
                    writeForecastInBase.Append(rf.humi.ToString() + " ");
                }
                var writeForecast = writeForecastInBase.ToString();
                var newWriteForecast = new Forecast { Prediction = writeForecast };
                db.Forecasts.Add(newWriteForecast);
                var userId = db.Users.Where(u => u.Login == User.Identity.Name).Select(u => u.Id).SingleOrDefault();
                var newRequest = new Request { UserId = userId };
                db.Requests.Add(newRequest);
                await db.SaveChangesAsync();
                newRequest.ForecastId = newWriteForecast.Id;
                await db.SaveChangesAsync();
                return View(FullForecast);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
        }
        //private bool representEqualDates(Archive archive, DayMonthYearResult res)
        //    => res.DayResult == archive.Day && res.MonthResult == archive.Month && res.YearResult == archive.Year;

        //private bool isMorning(Archive archive) => archive.Time == 0 || archive.Time == 3;
    }
}