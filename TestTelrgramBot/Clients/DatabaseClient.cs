using System.Net.Http.Json;
using System.Text;
using Newtonsoft.Json;
using TestTelrgramBot.Models;


namespace TestTelrgramBot.Clients
{
    public class DatabaseClient
    {
        private HttpClient _client = new HttpClient();

        public async Task<DatabaseModel?> GetFromDbAsync(string userId, string city)
        {
            string requestUri = $"https://travel-bot-api.herokuapp.com/InfoFromDB/get?id={userId}&city={city}";

            try
            {
                var result = await _client.GetAsync(requestUri);
                result.EnsureSuccessStatusCode();

                var model = result.Content.ReadAsStringAsync().Result;
                var infoDb = JsonConvert.DeserializeObject<DatabaseModel>(model);

                return result.IsSuccessStatusCode && infoDb != null ? infoDb : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<HttpResponseMessage?> DeleteAllRoutesAsync(string userId, string city)
        {
            string requestUri = $"https://travel-bot-api.herokuapp.com/InfoFromDB/delete?userId={userId}&city={city}";
            try
            {
                DeleteBody deleteBody = new DeleteBody()
                {
                    UserId = userId,
                    City = city
                };
                HttpRequestMessage request = new HttpRequestMessage
                {
                    Content = JsonContent.Create(deleteBody),
                    Method = HttpMethod.Delete,
                    RequestUri = new Uri(requestUri)
                };
                var result = await _client.SendAsync(request);
                return result.IsSuccessStatusCode ? result : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<string?> PostToDbAsync(DatabaseModel databaseModel)
        {
            string requestUri = "https://travel-bot-api.herokuapp.com/InfoFromDB/add";
            try
            {
                var json = JsonConvert.SerializeObject(databaseModel);
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                var post = await _client.PostAsync(requestUri, data);

                var postcontent = await post.Content.ReadAsStringAsync();
                return post.IsSuccessStatusCode && postcontent != null ? postcontent : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<string?> AddItemAsync(string userId, string city, string newRoute)
        {
            string requesrUri = "https://travel-bot-api.herokuapp.com/InfoFromDB/addItem";
            try
            {
                var person = new PutBody
                {
                    City = city,
                    UserId = userId,
                    NewRoute = newRoute
                };
                var response = await _client.PutAsJsonAsync(requesrUri, person);
                return response != null ? await response.Content.ReadAsStringAsync() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
        public async Task<string?> DeleteItemAsync(string userid, string city, string newRoute)
        {
            string requestUri = "https://travel-bot-api.herokuapp.com/InfoFromDB/deleteItem";
            try
            {
                var person = new PutBody
                {
                    City = city,
                    UserId = userid,
                    NewRoute = newRoute
                };
                var response = await _client.PutAsJsonAsync(requestUri, person);
                return response != null ? await response.Content.ReadAsStringAsync() : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }
    }

}
