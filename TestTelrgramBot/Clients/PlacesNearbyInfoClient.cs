using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestTelrgramBot.Constant;

namespace TestTelrgramBot.Clients
{
    internal class PlacesNearbyInfoClient
    {
        public async Task<PlacesNearbyInfoModel> GetInfoAsync(string lat, string lng, string type)
        {
            string url = $"https://travel-bot-api.herokuapp.com/PlaceNeary?lat={lat}&lng={lng}&type={type}";
            
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(url)
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(body);
                Console.ResetColor();

                PlacesNearbyInfoModel placesNearbyInfoModel = JsonConvert.DeserializeObject<PlacesNearbyInfoModel>(body);
                if (placesNearbyInfoModel.results != null)
                {
                    return placesNearbyInfoModel;
                }
                else
                {
                    Console.WriteLine("null 1");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("null 2");
                return null;
            }
        }
    }
}
