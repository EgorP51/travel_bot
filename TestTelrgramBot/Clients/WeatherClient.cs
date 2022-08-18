using TestTelrgramBot.Models;
using Newtonsoft.Json;
namespace TestTelrgramBot
{
    public class WeatherClient
    {
        public async Task<WeatherModel?> GetWeatherAsync(string city)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://travel-bot-api.herokuapp.com/WeatherControler?city={city}")
            };
            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                WeatherModel weatherModel = JsonConvert.DeserializeObject<WeatherModel>(body);
                bool isNull = weatherModel.forecasts != null && weatherModel.location != null;

                return !isNull ? weatherModel : null;
            }
            else
            {
                return null;
            }

        }
    }
}
