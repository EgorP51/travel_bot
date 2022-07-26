using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestTelrgramBot.Models;
using Newtonsoft.Json;
using TestTelrgramBot.Constant;

namespace TestTelrgramBot
{
    public class CityClient
    {
        public async Task<SearchInfoModel> GetCityInfoAsync(string city)
        {
            var client = new HttpClient(); // Wiki
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://travel-bot-api.herokuapp.com/SearchInfo?text={city}")
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                //json output to console
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine(body);
                Console.ResetColor();
                SearchInfoModel model = JsonConvert.DeserializeObject<SearchInfoModel>(body);
                if (model != null)
                {
                    return model;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }
    }
}
