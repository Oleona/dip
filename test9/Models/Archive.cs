using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace test9.Models
{
    public class WeatherContext : DbContext
    {
        public DbSet<Archive> Archives { get; set; }
        public DbSet<Extremum> Extrema { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<Forecast> Forecasts { get; set; }
    }

    public class Archive
    {

        // ID 
        public int Id { get; set; }
        // индекс станции
        public int? StationIndex { get; set; }
        // год по гринвичу
        public int? GreenwichYear { get; set; }
        // месяц по гринвичу
        public int? GreenwichMonth { get; set; }
        // день по гринвичу
        public int? GreenwichDay { get; set; }
        // время по гринвичу инт тк интервал 0 3 6 и тд
        public int? GreenwichMeanTime { get; set; }
        // год
        public int? Year { get; set; }
        // месяц
        public int? Month { get; set; }
        // день
        public int? Day { get; set; }
        // время инт тк интервал 0 3 6 и тд
        public int? Time { get; set; }
        // облачность
        public int? Cloudiness { get; set; }
        // индекс погоды интервал потом у него еще будет расшифровка но надеюсь обойтись ИФ елсе
        public int? Index { get; set; }
        // направление ветра
        public int? WindDirection { get; set; }
        // скорость ветра
        public int? WindSpeed { get; set; }
        // сумма осадков
        public double? PrecipitationSum { get; set; }
        // температура
        public double? Temperature { get; set; }
        // влажность
        public int? Humidity { get; set; }
        // давление
        public double? Pressure { get; set; }

    }

    public class Extremum
    {

        // ID экстремума
        public int Id { get; set; }
        // дата
        public string Day_Month { get; set; }
        // минимум
        public double Minimum { get; set; }
        // год минимума
        public int Minimum_Year { get; set; }
        // максимум температуры
        public double Maximum { get; set; }
        // год максимума
        public int Maximum_Year { get; set; }
        //  примета
        public string SignText { get; set; }

    }
    public class User
    {
        // ID 
        public int Id { get; set; }
        // логин
        public string Login { get; set; }
        // пароль
        public string Password { get; set; }

        public ICollection<Request> Requests { get; set; }

        //public User()
        //{
        //    Requests = new List<Request>();
        //}
    }
    public class Request

    {
        // ID 
        public int Id { get; set; }
        // id юзера
        public int UserId { get; set; }
        // id прогноза
        public int ForecastId { get; set; }
        // Id экстремума      
        public int ExtremumId { get; set; }


        public Forecast Forecast { get; set; }
        public User User { get; set; }

    }
    public class Forecast
    {
        [Key]
        [ForeignKey("Request")]

        // ID 
        public int Id { get; set; }
        // прогноз в виде текста
        public string Prediction { get; set; }

        public Request Request { get; set; }


    }

    public class OpenWeatherResponse
    {
        public Main Main { get; set; }
    }

    public class Main
    {
        public string Humidity { get; set; }
        public string Temp { get; set; }
        public string Pressure { get; set; }
    }

}