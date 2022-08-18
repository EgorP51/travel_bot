using TestTelrgramBot.Models;
using Newtonsoft.Json;

namespace TestTelrgramBot
{
    public class HotelsClient
    {
        public async Task<HotelModel?> GetHotelsAsync(string city, string checkin, string checkout, int adultsNumber)
        {
            string uri = $"https://travel-bot-api.herokuapp.com/CityHotel?city={city}&checkin={checkin}&checkout={checkout}&adults={adultsNumber.ToString()}";
            
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri(uri)
            };
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                HotelModel hotelModel = JsonConvert.DeserializeObject<HotelModel>(body);
                
                return hotelModel.results != null ? hotelModel : null;
            }
            else
            {
                return null;
            }
        }
    }
}
