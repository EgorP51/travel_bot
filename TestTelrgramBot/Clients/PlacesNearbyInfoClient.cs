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
			try
			{


				string url = $"https://travel-bot-api.herokuapp.com/PlaceNeary?lat={lat}&lng={lng}&type={type}";
				Console.WriteLine(url);
				var client = new HttpClient();
				var request = new HttpRequestMessage
				{
					Method = HttpMethod.Get,
					RequestUri = new Uri(url),
					Headers =
				{
					{ "X-RapidAPI-Key", Constants.ApiKey}
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
					PlacesNearbyInfoModel placesNearbyInfoModel = JsonConvert.DeserializeObject<PlacesNearbyInfoModel>(body);
					if (placesNearbyInfoModel.results != null)
					{
						return placesNearbyInfoModel;
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
			}catch (Exception ex)
            {
				return null;
            }
		}
	}
}
