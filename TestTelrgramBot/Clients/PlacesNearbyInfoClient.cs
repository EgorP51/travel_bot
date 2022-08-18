using Newtonsoft.Json;

namespace TestTelrgramBot.Clients
{
    internal class PlacesNearbyInfoClient
    {
        public async Task<PlacesNearbyInfoModel?> GetInfoAsync(string latitude, string longitude, string type)
        {
            string url = $"https://travel-bot-api.herokuapp.com/PlaceNeary?lat={latitude}&lng={longitude}&type={type}";
            
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
                PlacesNearbyInfoModel placesNearbyInfoModel = JsonConvert.DeserializeObject<PlacesNearbyInfoModel>(body);
                
                return placesNearbyInfoModel.results != null ? placesNearbyInfoModel : null;
            }
            else
            {
                return null;
            }
        }
    }
}
