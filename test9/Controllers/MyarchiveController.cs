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
        double tempe0_3, tempe6_9, tempe12_15, tempe18_21;
        int humi0_3, humi6_9, humi12_15, humi18_21;
        double pres0_3, pres6_9, pres12_15, pres18_21;
        int srok;

        const double k1 = 1.05;//  верхний коэффициент для границ выборки из базы. Подобран опытным путем
        const double k2 = 0.95; // нижний

        //12 октября

        double funkt;
        int funkh;
        double funkp;

        public List<ResultForecast> PartForecast(IEnumerable<Archive> f)
        {
            List<ResultForecast> resultForecast = new List<ResultForecast>();
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
        //12 октября конец

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

                // ниже старый блок

                // выбираем из базы значения температуры, влажности, давления, отличающиеся от погоды на данный момент на 
                // подобранный коэффициент с учетом времени ( не более часа в обе стороны разброс).
                var data = db.Archives
                    .Where(t => temp <= t.Temperature * k1 && temp >= t.Temperature * k2)
                    .Where(t => humidity <= t.Humidity * k1 && humidity >= t.Humidity * k2)
                    .Where(t => pressure <= t.Pressure * k1 && pressure >= t.Pressure * k2)
                    .Where(t => t.Time >= DateHour - 1 && t.Time <= DateHour + 1 ||
                                DateHour + 1 == 24 && t.Time == 0)
                    .ToList();


                //датаселект- хранит даты данных из data поэтому он в 3 раза длиннее- для каждого значения из data идет день месяц год
                // день сразу увеличиваем на 1 так как прогноз будет строиться по датам, следующим за имеющейся
                List<int> dayMonthYear = new List<int>();
                foreach (Archive b in data)
                {
                    dayMonthYear.Add((int)b.Day + 1);
                    dayMonthYear.Add((int)b.Month);
                    dayMonthYear.Add((int)b.Year);
                }
                List<Archive> OneDayForPrediction = new List<Archive>();
                List<Archive> forecast = new List<Archive>();
                for (int i = 0; i < dayMonthYear.Count; i += 3)
                {
                    var myDay = dayMonthYear[i];
                    var myMonth = dayMonthYear[i + 1];
                    var myYear = dayMonthYear[i + 2];
                    //OneDayForPrediction каждый раз содержит 8 значений- на одну дату по срокам 0-3-6-9-12-15-18-21 час
                    OneDayForPrediction = db.Archives.Where(v => v.Day == myDay && v.Month == myMonth && v.Year == myYear).ToList();

                    foreach (Archive n in OneDayForPrediction)
                    {
                        //forecast содержит все данные подходящих прогнозов на след день со сроками, чуть меньше чем 8*data.count т.к пренебрегла переходом на след.месяц
                        // для граничных дней типа 30 сент-31 сент не существует
                        forecast.Add(n);
                    }
                }

                List<Result> res = new List<Result>();
                int countTime0_3 = 0;//считает данные для прогоза на ночь- на 0 и 3 часа ночи
                int countTime6_9 = 0;
                int countTime12_15 = 0;
                int countTime18_21 = 0;
                foreach (Archive p in forecast)
                {
                    if (p.Time.HasValue)
                    {
                        if (p.Time == 0 || p.Time == 3)
                        {
                            if (p.Temperature.HasValue)
                            {
                                tempe0_3 += p.Temperature.Value;
                            }
                            if (p.Humidity.HasValue)
                            {
                                humi0_3 += p.Humidity.Value;
                            }

                            if (p.Pressure.HasValue)
                            {
                                pres0_3 += p.Pressure.Value;
                            }
                            countTime0_3++;
                        }
                        if (p.Time == 6 || p.Time == 9)
                        {
                            if (p.Temperature.HasValue)
                            {
                                tempe6_9 += p.Temperature.Value;
                            }
                            if (p.Humidity.HasValue)
                            {
                                humi6_9 += p.Humidity.Value;
                            }

                            if (p.Pressure.HasValue)
                            {
                                pres6_9 += p.Pressure.Value;
                            }
                            countTime6_9++;
                        }
                        if (p.Time == 12 || p.Time == 15)
                        {
                            if (p.Temperature.HasValue)
                            {
                                tempe12_15 += p.Temperature.Value;
                            }
                            if (p.Humidity.HasValue)
                            {
                                humi12_15 += p.Humidity.Value;
                            }

                            if (p.Pressure.HasValue)
                            {
                                pres12_15 += p.Pressure.Value;
                            }
                            countTime12_15++;
                        }
                        if (p.Time == 18 || p.Time == 21)
                        {
                            if (p.Temperature.HasValue)
                            {
                                tempe18_21 += p.Temperature.Value;
                            }
                            if (p.Humidity.HasValue)
                            {
                                humi18_21 += p.Humidity.Value;
                            }

                            if (p.Pressure.HasValue)
                            {
                                pres18_21 += p.Pressure.Value;
                            }
                            countTime18_21++;
                        }
                    }
                }
                if (countTime0_3 != 0)
                {

                    tempe0_3 = tempe0_3 / countTime0_3;
                    tempe0_3 = Math.Round(tempe0_3, 2);
                    humi0_3 = humi0_3 / countTime0_3;
                    pres0_3 = pres0_3 / countTime0_3;
                    pres0_3 = Math.Round(pres0_3, 2);
                    srok = 0;
                    Result result = new Result(tempe0_3, humi0_3, pres0_3, srok);
                    res.Add(result);
                }
                countTime0_3 = 0;

                if (countTime6_9 != 0)
                {
                    tempe6_9 = tempe6_9 / countTime6_9;
                    tempe6_9 = Math.Round(tempe6_9, 2);
                    humi6_9 = humi6_9 / countTime6_9;
                    pres6_9 = pres6_9 / countTime6_9;
                    pres6_9 = Math.Round(pres6_9, 2);
                    srok = 6;
                    Result result = new Result(tempe6_9, humi6_9, pres6_9, srok);
                    res.Add(result);
                }
                countTime6_9 = 0;

                if (countTime12_15 != 0)
                {
                    tempe12_15 = tempe12_15 / countTime12_15;
                    tempe12_15 = Math.Round(tempe12_15, 2);
                    humi12_15 = humi12_15 / countTime12_15;
                    pres12_15 = pres12_15 / countTime12_15;
                    pres12_15 = Math.Round(pres12_15, 2);
                    srok = 12;
                    Result result = new Result(tempe12_15, humi12_15, pres12_15, srok);
                    res.Add(result);
                }
                countTime12_15 = 0;

                if (countTime18_21 != 0)
                {
                    tempe18_21 = tempe18_21 / countTime18_21;
                    tempe18_21 = Math.Round(tempe18_21, 2);
                    humi18_21 = humi18_21 / countTime18_21;
                    pres18_21 = pres18_21 / countTime18_21;
                    pres18_21 = Math.Round(pres18_21, 2);
                    srok = 18;
                    Result result = new Result(tempe18_21, humi18_21, pres18_21, srok);
                    res.Add(result);
                }
                countTime18_21 = 0;
                ViewBag.Resul = res;



                //  12 октября пробую разбить сразу на 4 прогноза на день по времени суток

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
                List<Archive> forecastForMorningPrediction = new List<Archive>();
                List<Archive> forecastForDayPrediction = new List<Archive>();
                List<Archive> forecastForEveningPrediction = new List<Archive>();

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
                        .ToList().ForEach(it => forecastForMorningPrediction.Add(it));
                    db.Archives.Where(v => v.Day == myDayResult && v.Month == myMonthResult && v.Year == myYearResult && (v.Time == 12 || v.Time == 15))
                        .ToList().ForEach(it => forecastForDayPrediction.Add(it));
                    db.Archives.Where(v => v.Day == myDayResult && v.Month == myMonthResult && v.Year == myYearResult && (v.Time == 18 || v.Time == 21))
                        .ToList().ForEach(it => forecastForEveningPrediction.Add(it));
                }

                List<ResultForecast> FullForecast = new List<ResultForecast>();
                // Находим средние значения, как наш прогноз, для 4 частей суток, записываем в общий прогноз
                PartForecast(forecastForNightPrediction).ForEach(it => FullForecast.Add(it));
                PartForecast(forecastForMorningPrediction).ForEach(it => FullForecast.Add(it));
                PartForecast(forecastForDayPrediction).ForEach(it => FullForecast.Add(it));
                PartForecast(forecastForEveningPrediction).ForEach(it => FullForecast.Add(it));

                ViewBag.ResultFullForecast = FullForecast;
                //12 октября конец блока


                // готовим строку для записи результата прогноза в базу
                StringBuilder resultToBase = new StringBuilder();
                foreach (Result r in res)
                {
                    resultToBase.Append(r.tempe.ToString() + " ");
                    resultToBase.Append(r.pres.ToString() + " ");
                    resultToBase.Append(r.humi.ToString() + " ");
                }

                var forecastToBase = resultToBase.ToString();
                var newForecast = new Forecast { Prediction = forecastToBase };
                db.Forecasts.Add(newForecast);

                var userId = db.Users.Where(u => u.Login == User.Identity.Name).Select(u => u.Id).SingleOrDefault();
                var request = new Request { UserId = userId };
                db.Requests.Add(request);

                await db.SaveChangesAsync();

                request.ForecastId = newForecast.Id;
                await db.SaveChangesAsync();

                // 12 октября
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
                var newuserId = db.Users.Where(u => u.Login == User.Identity.Name).Select(u => u.Id).SingleOrDefault();
                var newrequest = new Request { UserId = newuserId };
                db.Requests.Add(newrequest);
                await db.SaveChangesAsync();
                newrequest.ForecastId = newForecast.Id;// нью или просто?
                await db.SaveChangesAsync();
                // 12 октября конец

                return View(res);
                //  return View(FullForecast);

            }
            else
            {

                return RedirectToAction("Login", "Home");
            }


        }
        private bool representEqualDates(Archive archive, DayMonthYearResult res)
            => res.DayResult == archive.Day && res.MonthResult == archive.Month && res.YearResult == archive.Year;

        private bool isMorning(Archive archive) => archive.Time == 0 || archive.Time == 3;
    }


}