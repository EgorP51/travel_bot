using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using TestTelrgramBot.Models;

namespace TestTelrgramBot.Clients
{
    public class DatabaseClient
    {
        private string apiAddress = "https://travel-bot-api.herokuapp.com/";
        private HttpClient client = new HttpClient();

        public async Task<DatabaseModel> GetFromDb(string userid,string city)
        {
            client.BaseAddress = new Uri(apiAddress);

            var result = await client.GetAsync($"InfoFromDB/get?id={userid}&city={city}");
            result.EnsureSuccessStatusCode();

            var model = result.Content.ReadAsStringAsync().Result;
            var infoDb = JsonConvert.DeserializeObject<DatabaseModel>(model);

            return infoDb;
        }

        public async Task<HttpResponseMessage> DeleteAllRoutes(string userId,string city)
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
                RequestUri = new Uri($"https://travel-bot-api.herokuapp.com/InfoFromDB/delete?userId={userId}&city={city}")
            };
            return await client.SendAsync(request);
        }

        public async Task<string> PostToDb(DatabaseModel databaseModel)
        {
            var json = JsonConvert.SerializeObject(databaseModel);

            var data = new StringContent(json, Encoding.UTF8, "application/json");

            var post = await client.PostAsync("https://travel-bot-api.herokuapp.com/InfoFromDB/add", data);

            var postcontent = post.Content.ReadAsStringAsync().Result;

            return postcontent;
        }

        public async Task<string> AddItem(string userid, string city,string newRoute)
        {
            var person = new PutBody
            {
                City = city,
                UserId = userid,
                NewRoute = newRoute
            };

            var json = JsonConvert.SerializeObject(person);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            client.BaseAddress = new Uri("https://travel-bot-api.herokuapp.com/InfoFromDB/");
            var response = client.PutAsJsonAsync("https://travel-bot-api.herokuapp.com/InfoFromDB/addItem", person ).Result;

            return response.Content.ReadAsStringAsync().Result;
        }


        public async Task<string> DeleteItem(string userid, string city, string newRoute)
        {
            var person = new PutBody
            {
                City = city,
                UserId = userid,
                NewRoute = newRoute
            };

            var json = JsonConvert.SerializeObject(person);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            client.BaseAddress = new Uri("https://travel-bot-api.herokuapp.com/InfoFromDB/");
            var response = client.PutAsJsonAsync("https://travel-bot-api.herokuapp.com/InfoFromDB/deleteItem", person).Result;

            return response.Content.ReadAsStringAsync().Result;
        }

    }
    public class PutBody
    {
        public string UserId { get; set; }
        public string City { get; set; }
        public string NewRoute { get; set; }
    }
    public class DeleteBody
    {
        public string UserId { get; set; }
        public string City { get; set; }
    }

}
