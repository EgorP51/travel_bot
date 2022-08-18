using TestTelrgramBot.Clients;
using TestTelrgramBot.Models;
using Telegram.Bot.Types;
using Telegram.Bot;


namespace TestTelrgramBot
{
    class ApiHandler
    {
        private ITelegramBotClient _botClient;
        private CancellationToken _cancellationToken;
        private Message _message;
        private WeatherModel _weatherModel;
        private HotelModel _hotelsList;
        private SearchInfoModel _searchInfoModel;
        private PlacesNearbyInfoModel _placeNearbyInfoModel;
        private string _checkout;
        private int _adults;
        private string _checkin { get; set; }
        public List<HotelMessageModel> HotelMessageModels { get; set; }
        public WeatherMessageModel WeatherMessageModel { get; set; }
        public int ApiHandlerMessageId { get; set; }
        public CityMessageModel CityMessageModel { get; set; }
        public string City { get; set; }
        public string UserId { get; set; }
        public static int N { get; set; } = 0;

        public ApiHandler(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message, string city)
        {
            _botClient = botClient;
            _cancellationToken = cancellationToken;
            _message = message;
            City = city;

            _searchInfoModel = new CityClient().GetCityInfoAsync(city).Result; // city information from json
            _weatherModel = new WeatherClient().GetWeatherAsync(city).Result;  // weather information from json

            if (_searchInfoModel != null && _weatherModel != null)
            {
                WeatherMessageModel = WeatherMessendgeAsync().Result;
                CityMessageModel = CityMessageAsync().Result; // receive a ready-made message about the city
            }
            if (CityMessageModel != null)
            {
                CityMessageModel.WeatherMessageModel = WeatherMessageModel;
            }
        }

        public bool CorrectlyEnteredCity(string text)
        {
            // Method for checking the correctness of the entered city name.
            // If the name entered and the name returned by the api are more
            // than 60% similar, the check is passed.
            if (_weatherModel != null && _searchInfoModel != null)
            {
                bool isCorrect = Compute(_weatherModel.location.city.ToLower(), _searchInfoModel.title.ToLower()) > 60 &&
                                 Compute(_weatherModel.location.city.ToLower(), text.ToLower()) > 60 &&
                                 Compute(text.ToLower(), _searchInfoModel.title.ToLower()) > 60;

                return isCorrect;
            }
            else
            {
                return false;
            }
        }
        private double Compute(string firstWord, string secondWord)
        {
            int firstLength = firstWord.Length;
            int secondLength = secondWord.Length;

            double final = firstLength > secondLength ? firstLength : secondLength;

            int[,] d = new int[firstLength + 1, secondLength + 1];

            for (int i = 0; i <= firstLength; d[i, 0] = i++) ;
            for (int j = 1; j <= secondLength; d[0, j] = j++) ;

            for (int i = 1; i <= firstLength; i++)
            {
                for (int j = 1; j <= secondLength; j++)
                {
                    int cost = (secondWord[j - 1] == firstWord[i - 1]) ? 0 : 1;
                    int min1 = d[i - 1, j] + 1;
                    int min2 = d[i, j - 1] + 1;
                    int min3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(min1, min2), min3);
                }
            }
            return (final - Convert.ToDouble(d[firstLength, secondLength])) / final * 100;
        }

        public async Task<List<PlacesNearbyInfoMassegeModel>> PlacesNearbyMessageAsync(string latitude, string longitude, string type)
        {
            // the method returns information about places near the selected hotel from api
            _placeNearbyInfoModel = await new PlacesNearbyInfoClient().GetInfoAsync(latitude, longitude, type);
            List<PlacesNearbyInfoMassegeModel> placesNearbyInfoMassegeModels = new List<PlacesNearbyInfoMassegeModel>();

            for (int i = 0; i < _placeNearbyInfoModel.results.Length; i++)
            {
                placesNearbyInfoMassegeModels.Add
                (
                    new PlacesNearbyInfoMassegeModel(_botClient, _cancellationToken, _message)
                    {
                        Latitude = _placeNearbyInfoModel.results[i].location.lat,
                        Longitude = _placeNearbyInfoModel.results[i].location.lng,
                        Name = _placeNearbyInfoModel.results[i].name,
                        Address = _placeNearbyInfoModel.results[i].address == null ? "" : _placeNearbyInfoModel.results[i].address,
                        Phone_number = _placeNearbyInfoModel.results[i].phone_number,
                        Numb = i
                    }
                );
            }
            return placesNearbyInfoMassegeModels;
        }

        public async Task<CityMessageModel?> CityMessageAsync()
        {
            // the method returns information about cities near the selected hotel from api
            bool modelIsEmpty = _searchInfoModel.title == null && _searchInfoModel.url == null &&
                _searchInfoModel.summary[0] == null && _searchInfoModel.image == null;

            if (!modelIsEmpty)
            {
                try
                {
                    string firstText = _searchInfoModel.summary[0].Length > _searchInfoModel.summary[1].Length ? _searchInfoModel.summary[0] : _searchInfoModel.summary[1];
                    string secondText = _searchInfoModel.summary[2].Length > firstText.Length ? _searchInfoModel.summary[2] : firstText;

                    CityMessageModel = new CityMessageModel(City, _botClient, _cancellationToken, _message)
                    {
                        Title = _searchInfoModel.title,
                        Body = secondText,
                        InfoUrl = _searchInfoModel.url,
                        MainPhoto = _searchInfoModel.image

                    };
                    CityMessageModel.Message = _message;
                    CityMessageModel.Latitude = _weatherModel.location.lat;
                    CityMessageModel.Longitude = _weatherModel.location.@long;
                    CityMessageModel.Title = _searchInfoModel.title;
                    CityMessageModel.InfoUrl = _searchInfoModel.url;
                    
                    return CityMessageModel;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
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
            HotelMessageModels = new List<HotelMessageModel>();
            HotelMessageModels = await HotelsListAsync(date);

            if (HotelMessageModels == null)
            {
                await _botClient.SendStickerAsync(_message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                await _botClient.SendTextMessageAsync(_message.Chat.Id, $"There was a data entry error");
                return;
            }
            else
            {
                foreach (var mess in HotelMessageModels)
                {
                    await mess.GetHotelMessageModelAsync(_message);
                }
            }
        }

        public async Task<WeatherMessageModel> WeatherMessendgeAsync()
        {
            string weather = "";
            string weatherSticker = "";

            if (_weatherModel != null)
            {
                weather += _weatherModel.location.city + "\n" + _weatherModel.location.country + "\n";
                foreach (var weatherDay in _weatherModel.forecasts)
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
            WeatherMessageModel weatherMessageModel = new WeatherMessageModel(_botClient)
            {
                forecasts = weather,
                mId = ApiHandlerMessageId
            };
            return weatherMessageModel;
        }

        public async Task<List<HotelMessageModel>?> HotelsListAsync(string date)
        {
            _checkin = date.Split(',')[0];
            _checkout = date.Split(',')[1];
            _adults = int.Parse(date.Split(',')[2]);

            _hotelsList = await new HotelsClient().GetHotelsAsync(City, _checkin, _checkout, _adults);

            if (_hotelsList == null)
                return null;
            else if (HotelMessageModels == null)
                return null;

            for (int i = 0; i < _hotelsList.results.Length; i++)
            {
                HotelMessageModels.Add(new HotelMessageModel(_botClient, _cancellationToken, i)
                {
                    Images = _hotelsList.results[i].images,
                    Name = _hotelsList.results[i].name,
                    Bathrooms = _hotelsList.results[i].bathrooms,
                    Bedrooms = _hotelsList.results[i].bedrooms,
                    Beds = _hotelsList.results[i].beds,
                    Lat = _hotelsList.results[i].lat,
                    Lng = _hotelsList.results[i].lng,
                    Uri = _hotelsList.results[i].url,
                    PreviewAmenities = _hotelsList.results[i].previewAmenities,
                    Total = _hotelsList.results[i].price.total,
                    Rating = _hotelsList.results[i].rating,
                    MId = ApiHandlerMessageId,
                    Numb = i
                });
            }
            return HotelMessageModels;
        }
    }
}
