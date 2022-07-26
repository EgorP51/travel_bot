using Telegram.Bot.Types.ReplyMarkups;using System;
using System.Collections.Generic;
using TestTelrgramBot.Clients;
using Telegram.Bot.Extensions;
using TestTelrgramBot.Models;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Newtonsoft.Json;
using Telegram.Bot;
using System.Linq;
using System.Text;
using System;

namespace TestTelrgramBot
{
    class ApiHandler
    {
        public ITelegramBotClient botClient { get; set; }
        public CancellationToken cancellationToken { get; set; }
        public Message message { get; set; }

        public List<HotelMessageModel> hotelMessageModels { get; set; }
        public WeatherModel weatherModel { get; set; }
        public HotelModel hotelsList { get; set; }
        public SearchInfoModel searchInfoModel { get; set; }
        public PlacesNearbyInfoModel placeNearbyInfoModel { get; set; }
        public static int n { get; set; } = 0;

        public PlacesNearbyInfoMassegeModel placeNearbyInfoMassegeModel { get; set; }
        public CityMessageModel cityMessageModel { get; set; }
        public WeatherMessageModel weatherMessageModel { get; set; }

        public int apiHandlerMessageId { get; set; }
        public string city { get; set; }
        string checkin { get; set; }
        string checkout { get; set; }
        int adults { get; set; }
        public string userId { get; set; }

        public ApiHandler(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message, string city)
        {
            this.botClient = botClient;
            this.cancellationToken = cancellationToken;
            this.message = message;
            this.city = city;

            searchInfoModel = new CityClient().GetCityInfoAsync(city).Result; // city information from json
            weatherModel = new WeatherClient().GetWeatherAsync(city).Result;  // weather information from json

            if (searchInfoModel != null && weatherModel != null)
            {
                weatherMessageModel = WeatherMessendge().Result;
                cityMessageModel = CityMessage().Result; // receive a ready-made message about the city
            }
            if (cityMessageModel != null)
            {
                cityMessageModel.weatherMessageModel = weatherMessageModel;
            }
        }

        public bool CorrectlyEnteredCity(string text)
        {
            //method for checking the correctness of the entered city name.
            //If the name entered and the name returned by the api are more
            //than 60% similar, the check is passed

            if (weatherModel != null && searchInfoModel != null)
            {
                if (Compute(weatherModel.location.city.ToLower(), searchInfoModel.title.ToLower()) > 60 &&
                    Compute(weatherModel.location.city.ToLower(), text.ToLower()) > 60 &&
                    Compute(text.ToLower(), searchInfoModel.title.ToLower()) > 60)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
        private double Compute(string s, string t)
        {
            int n = s.Length;
            int m = t.Length;

            double final = n > m ? n : m;

            int[,] d = new int[n + 1, m + 1];

            for (int i = 0; i <= n; d[i, 0] = i++) ;
            for (int j = 1; j <= m; d[0, j] = j++) ;

            for (int i = 1; i <= n; i++)
            {
                for (int j = 1; j <= m; j++)
                {
                    int cost = (t[j - 1] == s[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return (final - Convert.ToDouble(d[n, m])) / final * 100;
        }

        public async Task<List<PlacesNearbyInfoMassegeModel>> PlacesNearbyMessage(string lat, string lng, string type)
        {
            // the method returns information about places near the selected hotel from api
                        placeNearbyInfoModel = new PlacesNearbyInfoClient().GetInfoAsync(lat, lng, type).Result;
            List<PlacesNearbyInfoMassegeModel> placesNearbyInfoMassegeModels = new List<PlacesNearbyInfoMassegeModel>();

            for (int i = 0; i < placeNearbyInfoModel.results.Length; i++)
            {
                placesNearbyInfoMassegeModels.Add
                (
                    new PlacesNearbyInfoMassegeModel(botClient, cancellationToken, message)
                    {
                        //latitude = double.Parse(placeNearbyInfoModel.results[i].location.lat.ToString().Replace('.', ',')),
                        //longitude = double.Parse(placeNearbyInfoModel.results[i].location.lng.ToString().Replace('.', ',')),
                        latitude = placeNearbyInfoModel.results[i].location.lat,
                        longitude = placeNearbyInfoModel.results[i].location.lng,
                        name = placeNearbyInfoModel.results[i].name,
                        address = placeNearbyInfoModel.results[i].address == null ? "" : placeNearbyInfoModel.results[i].address,
                        phone_number = placeNearbyInfoModel.results[i].phone_number,
                        distance = placeNearbyInfoModel.results[i].distance,
                        numb = i
                    }
                );
            }
            return placesNearbyInfoMassegeModels;
        }

        public async Task<CityMessageModel> CityMessage()
        {
            // the method returns information about cities near the selected hotel from api
            if (searchInfoModel.title != null && searchInfoModel.url != null &&
                searchInfoModel.summary[0] != null && searchInfoModel.image != null)
            {
                try
                {
                    string firstText = searchInfoModel.summary[0].Length > searchInfoModel.summary[1].Length ? searchInfoModel.summary[0] : searchInfoModel.summary[1];
                    string secondText = searchInfoModel.summary[2].Length > firstText.Length ? searchInfoModel.summary[2] : firstText;

                    cityMessageModel = new CityMessageModel(city, botClient, cancellationToken, message)
                    {
                        title = searchInfoModel.title,
                        body = secondText,
                        infoUrl = searchInfoModel.url,
                        mainPhoto = searchInfoModel.image

                    };
                    cityMessageModel.message = message;
                    cityMessageModel.latitude = weatherModel.location.lat;
                    cityMessageModel.longitude = weatherModel.location.@long;
                    cityMessageModel.title = searchInfoModel.title;
                    cityMessageModel.infoUrl = searchInfoModel.url;
                    return cityMessageModel;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        public async Task GetHotelAsync(string date)
        {
            hotelMessageModels = new List<HotelMessageModel>();
            hotelMessageModels = await HotelsList(date);

            if (hotelMessageModels == null)
            {
                await botClient.SendStickerAsync(message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                await botClient.SendTextMessageAsync(message.Chat.Id, $"There was a data entry error");
                return;
            }
            else
            {
                foreach (var mess in hotelMessageModels)
                {
                    mess.GetHotelMessageModel(message);
                }
            }
        }

        public async Task<WeatherMessageModel> WeatherMessendge()
        {
            string weather = "";
            string weatherSticker = "";

            if (weatherModel != null)
            {
                weather += weatherModel.location.city + "\n" + weatherModel.location.country + "\n";
                foreach (var weatherDay in weatherModel.forecasts)
                {
                    switch (weatherDay.text)
                    {
                        case "Sunny":
                            weatherSticker = "☀";
                            break;
                        case "Mostly Sunny":
                            weatherSticker = "🌤";
                            break;
                        case "Cloudy":
                            weatherSticker = "☁️";
                            break;
                        case "Rain":
                            weatherSticker = "🌧";
                            break;
                        case "Partly Cloudy":
                            weatherSticker = "🌥";
                            break;
                        case "Scattered Thunderstorms":
                            weatherSticker = "🌩";
                            break;
                        case "Thunderstorms":
                            weatherSticker = "⚡️";
                            break;
                        case "Breezy":
                            weatherSticker = "☁️❄️";
                            break;
                        case "Scattered Showers":
                            weatherSticker = "🌨";
                            break;
                        case "Mostly Cloudy":
                            weatherSticker = "🌥";
                            break;
                        default:
                            weatherSticker = "❗️";
                            break;
                    }

                    weather += $"{weatherDay.day.ToUpper()}: min {weatherDay.low} °С - max {weatherDay.high} °С\n" +
                               $"Short description: {weatherDay.text} {weatherSticker}\n" +
                               $"______________________________________\n";
                }
            }
            else
            {
                weather = "Problem with data entry :)";
            }
            WeatherMessageModel weatherMessageModel = new WeatherMessageModel(botClient, cancellationToken, message)
            {
                forecasts = weather,
                mId = apiHandlerMessageId
            };
            return weatherMessageModel;
        }

        public async Task<List<HotelMessageModel>> HotelsList(string date)
        {
            checkin = date.Split(',')[0]; 
            checkout = date.Split(',')[1];
            adults = int.Parse(date.Split(',')[2]);

            hotelsList = await new HotelsClient().GetHotelsAsync(city, checkin, checkout, adults);

            if (hotelsList == null)
            {
                return null;
            }
            else if (hotelMessageModels == null)
            {
                return null;
            }
            for (int i = 0; i < hotelsList.results.Length; i++)
            {
                hotelMessageModels.Add(new HotelMessageModel(botClient, cancellationToken, message, i)
                {
                    images = hotelsList.results[i].images,
                    name = hotelsList.results[i].name,
                    bathrooms = hotelsList.results[i].bathrooms,
                    bedrooms = hotelsList.results[i].bedrooms,
                    beds = hotelsList.results[i].beds,
                    lat = hotelsList.results[i].lat,
                    lng = hotelsList.results[i].lng,
                    url = hotelsList.results[i].url,
                    previewAmenities = hotelsList.results[i].previewAmenities,
                    total = hotelsList.results[i].price.total,
                    rating = hotelsList.results[i].rating,
                    mId = apiHandlerMessageId,
                    numb = i
                });
            }
            return hotelMessageModels;
        }
    }
}
