using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTelrgramBot.Models;
using Newtonsoft.Json;

namespace TestTelrgramBot
{
    public class HotelsClient
    {
        public async Task<HotelModel> GetHotelsAsync(string city, string checkin, string checkout, int adults)
        {
            string uri = $"https://travel-bot-api.herokuapp.com/CityHotel?city={city}&checkin={checkin}&checkout={checkout}&adults={adults.ToString()}";
            Console.WriteLine(uri);
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri),
                Headers =
                {
                    { "X-RapidAPI-Key", "225f8c7ff7mshb7e7f4e4c0c08f4p141e90jsn8eecb2014b0b" },
                    { "X-RapidAPI-Host", "airbnb13.p.rapidapi.com" },
                },
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(body);
                Console.ResetColor();
                HotelModel hotelModel = JsonConvert.DeserializeObject<HotelModel>(body);
                if (hotelModel.results != null)
                {
                    return hotelModel;
                }
                else
                {

                    Console.WriteLine("null1");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("null1");
                return null;
            }
        }
    }
}
