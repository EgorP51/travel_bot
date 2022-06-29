using Newtonsoft.Json;
using TestTelrgramBot.Models;
using TestTelrgramBot;


AbungaBangaBot abungaBangaBot = new AbungaBangaBot();
abungaBangaBot.Start();
//Console.ReadKey();
while (true)
{

}














































//var client = new HttpClient();
//var request = new HttpRequestMessage
//{
//	Method = HttpMethod.Get,
//	RequestUri = new Uri("https://yahoo-weather5.p.rapidapi.com/weather?location=rome&format=json&u=c"),
//	Headers =
//	{
//		{ "X-RapidAPI-Host", "yahoo-weather5.p.rapidapi.com" },
//		{ "X-RapidAPI-Key", "225f8c7ff7mshb7e7f4e4c0c08f4p141e90jsn8eecb2014b0b" },
//	},
//};
//using (var response = await client.SendAsync(request))
//{
//	response.EnsureSuccessStatusCode();
//	var body = await response.Content.ReadAsStringAsync();
//	Console.WriteLine(body);
//}




































//string city = "kyiv";
//var client = new HttpClient();
//var request = new HttpRequestMessage
//{
//Method = HttpMethod.Get,
//RequestUri = new Uri($"https://hotel-price-aggregator.p.rapidapi.com/search?q={city}"),
//Headers =
//    {
//        { "X-RapidAPI-Host", "hotel-price-aggregator.p.rapidapi.com" },
//        { "X-RapidAPI-Key", "225f8c7ff7mshb7e7f4e4c0c08f4p141e90jsn8eecb2014b0b" },
//    },
//};
//using (var response = await client.SendAsync(request))
//{
//response.EnsureSuccessStatusCode();
//var body = await response.Content.ReadAsStringAsync();
////string text = body.Trim('[', ']');
//HotelModel[] hotels = JsonConvert.DeserializeObject<HotelModel[]>(body);
//    for (int i = 0; i < hotels.Length; i++)
//    {
//        var hotel = hotels[i];
//        Console.WriteLine("Name: " + hotel.name);
//        Console.WriteLine("Id: " + hotel.hotelId);
//        Console.WriteLine("Address: " + hotel.address.address + " " + hotel.address.country + " " + hotel.address.address);
//        Console.WriteLine("Coor: " + hotel.coordenates);
//        Console.WriteLine();
//    }
//}

//public class HotelModel
//{
//    public string name { get; set; }

//    public Address address { get; set; }
//    public string coordenates { get; set; }
//    public string hotelId { get; set; }

//}
//public class Address
//{
//    public string city { get; set; }
//    public string country { get; set; }
//    public string address { get; set; }
//}


//var client = new HttpClient();
//var request = new HttpRequestMessage
//{
//	Method = HttpMethod.Get,
//	RequestUri = new Uri("https://contextualwebsearch-websearch-v1.p.rapidapi.com/api/Search/WebSearchAPI?q=Rome&pageNumber=1&pageSize=10&autoCorrect=true"),
//	Headers =
//	{
//		{ "X-RapidAPI-Host", "contextualwebsearch-websearch-v1.p.rapidapi.com" },
//		{ "X-RapidAPI-Key", "225f8c7ff7mshb7e7f4e4c0c08f4p141e90jsn8eecb2014b0b" },
//	},
//};
//using (var response = await client.SendAsync(request))
//{
//	response.EnsureSuccessStatusCode();
//	var body = await response.Content.ReadAsStringAsync();
//	Console.WriteLine(body);
//}