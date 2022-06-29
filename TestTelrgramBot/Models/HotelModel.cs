using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestTelrgramBot.Models
{
    public class HotelModel
    {
        public Results[] results { get; set; }
    }
    public class Results
    {
        public string id { get; set; }
        public string url { get; set; }
        public string deeplink { get; set; }
        public string name { get; set; }
        public string bathrooms { get; set; }
        public int bedrooms { get; set; }
        public int beds { get; set; }
        public string neighborhood { get; set; }
        public string[] images { get; set; }
        public string hostThumbnail { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int person { get; set; }
        public float rating { get; set; }
        public string address { get; set; }
        public string[] previewAmenities { get; set; }
        public Price price { get; set; }

    }
    public class Price
    {
        public int rate { get; set; }
        public string currency { get; set; }
        public int total { get; set; }
        public PriceItems[] priceItems { get; set; }
    }
    public class PriceItems
    {
        public string title { get; set; }
        public string amount { get; set; }
    }
}
