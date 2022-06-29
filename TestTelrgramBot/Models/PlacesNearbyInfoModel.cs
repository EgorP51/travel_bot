using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTelrgramBot
{
    public class PlacesNearbyInfoModel
    {
        public Result[] results { get; set; }
    }
    public class Result
    {
        public string name { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public Loc location { get; set; }
        public string distance { get; set; }
    }
    public class Loc
    {
        public double lat { get; set; }
        public double lng { get; set; }
    }
}
