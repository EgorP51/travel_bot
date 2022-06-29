using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTelrgramBot.Models
{
    public class WeatherModel
    {
        public Forecasts[] forecasts { get; set; }
        public Location location { get; set; }
    }
    public class Location
    {
        public string city { get; set; }
        public string country { get; set; }
        public float lat { get; set; }
        public float @long { get; set; }
    }
    public class Forecasts
    {
        public string day { get; set; }
        public string low { get; set; }
        public string high { get; set; }
        public string text { get; set; }
    }

}
