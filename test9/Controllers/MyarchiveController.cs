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
        double tempe0_3, tempe6_9, tempe12_15, tempe18_21;
        int humi0_3, humi6_9, humi12_15, humi18_21;
        double pres0_3, pres6_9, pres12_15, pres18_21;
        int srok;
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
               
                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.GetAsync("https://api.openweathermap.org/data/2.5/weather?q=%20Saint%20Petersburg,ru&appid=c986959d1fd2ecb4376e73e676671d28&units=metric");
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
                var archive = db.Archives.Where(t => (temp <= (t.Temperature * 1.05)) && (humidity <= (t.Humidity * 1.05)) && (pressure <= (t.Pressure * 1.05))).FirstOrDefault();
               // ViewBag.Archive = archive;
                List<Archive> data = db.Archives.Where(t =>
                (temp <= (t.Temperature * 1.05) && temp >= (t.Temperature * 0.95)
                && (humidity <= (t.Humidity * 1.05) && humidity >= t.Humidity * 0.95)
                && (pressure <= (t.Pressure * 1.05) && pressure >= t.Pressure * 0.95)
                && (DateHour <= t.Time + 1) && (DateHour >= t.Time - 1)
                )).ToList();
                //датаселект- хранит даты данных из data поэтому он в 3 раза длиннее- для каждого значения из data идет день месяц год
                List<int> dataSelect = new List<int>();
                foreach (Archive b in data)

                {
                    dataSelect.Add((int)b.Day + 1);
                    dataSelect.Add((int)b.Month);
                    dataSelect.Add((int)b.Year);
                }
                List<Archive> data1 = new List<Archive>();
                List<Archive> data2 = new List<Archive>();
                for (int i = 0; i < dataSelect.Count; i += 3)
                {
                    var myDay = dataSelect[i];
                    var myMonth = dataSelect[i + 1];
                    var myYear = dataSelect[i + 2];
                    var prognozList = db.Archives.Where(v => v.Day == myDay && v.Month == myMonth && v.Year == myYear);
                    //data1 каждый раз содержит 8 значений- на дату по срокам
                    data1 = prognozList.ToList();
                    foreach (Archive n in data1)
                    {
                        //data2 содержит все данные подходящих прогнозов на след день со сроками чуть меньше чем 8*data.count т.к пренебрегла переходом на след.месяц
                        // для граничных дней типа 30 сент-31 сент не существует
                        data2.Add(n);
                    }
                }
                
                List<Result> res = new List<Result>();
                int countTime0_3 = 0;//считает интервалы от 0 до 3 часов ночи
                int countTime6_9 = 0;
                int countTime12_15 = 0;
                int countTime18_21 = 0;
                foreach (Archive p in data2)
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
                    humi0_3 = humi0_3 / countTime0_3;
                    pres0_3 = pres0_3 / countTime0_3;
                    srok = 0;
                    Result result = new Result(tempe0_3, humi0_3, pres0_3, srok);
                    res.Add(result);
                }
                countTime0_3 = 0;
                
                if (countTime6_9 != 0)
                {
                    tempe6_9 = tempe6_9 / countTime6_9;
                    humi6_9 = humi6_9 / countTime6_9;
                    pres6_9 = pres6_9 / countTime6_9;
                    srok = 6;
                    Result result = new Result(tempe6_9, humi6_9, pres6_9, srok);
                    res.Add(result);
                }
                countTime6_9 = 0;

                if (countTime12_15 != 0)
                {
                    tempe12_15 = tempe12_15 / countTime12_15;
                    humi12_15 = humi12_15 / countTime12_15;
                    pres12_15 = pres12_15 / countTime12_15;
                    srok = 12;
                    Result result = new Result(tempe12_15, humi12_15, pres12_15, srok);
                    res.Add(result);
                }
                countTime12_15 = 0;

                if (countTime18_21 != 0)
                {
                    tempe18_21 = tempe18_21 / countTime18_21;
                    humi18_21 = humi18_21 / countTime18_21;
                    pres18_21 = pres18_21 / countTime18_21;
                    srok = 18;
                    Result result = new Result(tempe18_21, humi18_21, pres18_21, srok);
                    res.Add(result);
                }
                countTime18_21 = 0;
                ViewBag.Resul = res;
                // return View(archive);
                return View(res);
            }
            else
            {

                return RedirectToAction("Login", "Home");
            }

        }

    }
}