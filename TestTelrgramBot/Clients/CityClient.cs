using TestTelrgramBot.Models;
using Newtonsoft.Json;

namespace TestTelrgramBot
{
    public class CityClient
    {
        public async Task<SearchInfoModel?> GetCityInfoAsync(string city)
        {
            var client = new HttpClient();
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
                SearchInfoModel model = JsonConvert.DeserializeObject<SearchInfoModel>(body);

                return model != null ? model : null;
            }
            else
            {
                return null;
            }
        }
    }
}
