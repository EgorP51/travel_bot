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
    public class WeatherClient
    {
        public async Task<WeatherModel> GetWeatherAsync(string city)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://travel-bot-api.herokuapp.com/WeatherControler?city={city}"),
                Headers =
                {
                    { "X-RapidAPI-Host", "hotels4.p.rapidapi.com" },
                    { "X-RapidAPI-Key", Constants.ApiKey},
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
                WeatherModel weatherModel = JsonConvert.DeserializeObject<WeatherModel>(body);
                if (weatherModel == null)
                    return null;
                if (weatherModel.forecasts != null && weatherModel.location != null)
                {
                    return weatherModel;
                }
                else
                {
                    Console.WriteLine("null1");
                    return null;
                }
            }
            else
            {
                Console.WriteLine("null2");
                return null;
            }

        }
    }
}
