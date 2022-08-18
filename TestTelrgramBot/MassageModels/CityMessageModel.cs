using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestTelrgramBot
{
    public class CityMessageModel
    {
        private string _city;
        private ITelegramBotClient _botClient;
        private CancellationToken _cancellationToken;
        public WeatherMessageModel WeatherMessageModel { get; set; }
        public Message Message { get; set; }
        public string MainPhoto { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string InfoUrl { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public int MId { get; set; }


        public CityMessageModel(string city, ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)
        {
            _city = city;
            _botClient = botClient;
            _cancellationToken = cancellationToken;
        }

        public async Task<int> GetCityMessageModelAsync(Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new
            (
               new[]
               {
                    new []
                    {
                        InlineKeyboardButton.WithUrl(text: "More info ℹ️", InfoUrl),
                        InlineKeyboardButton.WithCallbackData("Weather ☀️",callbackData: "CityWeather")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "List of hotels 🏨", callbackData: $"CityHotels"),
                        InlineKeyboardButton.WithCallbackData(text: "List of useful links 🗺✈️🖥", callbackData: $"CityLinks")
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Show on the map 🗺", callbackData: $"CityShowOnTheMap")
                    }
               }
            );
            Message tempMesssage;
            try
            {
                tempMesssage = await _botClient.SendPhotoAsync
                (
                    chatId: message.Chat.Id,
                    photo: MainPhoto,
                    caption: $"{Title}\n{Body}",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: _cancellationToken
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                tempMesssage = await _botClient.SendPhotoAsync
                (
                   message.Chat.Id,
                   photo: "https://www.salonlfc.com/wp-content/uploads/2018/01/image-not-found-scaled-1150x647.png",
                   caption: $"(city ​​photos not found)\n{Title}\n{Body}",
                   replyMarkup: inlineKeyboard,
                   cancellationToken: _cancellationToken
                );
            }

            return tempMesssage.MessageId;
        }

        public async Task CityHandlerCallbackQueryAsync(CallbackQuery callbackQuery)
        {
            if (callbackQuery.Data == "CityWeather")
            {
                await WeatherMessageModel.GetWeatherMessageModel(Message);
                return;
            }
            else if (callbackQuery.Data == "CityHotels")
            {
                await _botClient.SendTextMessageAsync(Message.Chat.Id,
                    "Enter the date of check-in, check-out and the number " +
                    "of people in this format: 2022-07-13,2022-07-16,2");
                return;
            }
            else if (callbackQuery.Data == "CityLinks")
            {
                InlineKeyboardMarkup inlineKeyboard = new
             (
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Skyscanner","https://www.skyscanner.net/?previousCultureSource=GEO_LOCATION&redirectedFrom=www.skyscanner.com"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Funjet","https://www.funjet.com/"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Tripadvis","https://www.tripadvisor.com/"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Кayak","https://www.kayak.co.uk/flights"),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Booking.com","https://www.booking.com/index.en-gb.html?label=gen173rf-1FCAEoggI46AdIM1gDaFCIAQGYASG4ARfIAQzYAQHoAQH4AQuIAgGiAgxyYXBpZGFwaS5jb22oAgO4As_b65UGwAIB0gIkOTVlNjlhODUtM2ViOS00ZDYyLWFjNjctZTIyZDJlMDdkMzBm2AIG4AIB&sid=993b9c3ae029078553280e773255d7d8&lang=en-gb&sb_price_type=total&soz=1&lang_click=other&cdl=ru&lang_changed=1"),
                    },
                }
             );
                await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Should help)))", replyMarkup: inlineKeyboard);
                return;
            }
            else if (callbackQuery.Data == "CityShowOnTheMap")
            {
                await _botClient.SendVenueAsync
                (
                    chatId: callbackQuery.Message.Chat.Id,
                    latitude: Latitude,
                    longitude: Longitude,
                    title: _city,
                    address: Title,
                    cancellationToken: _cancellationToken,
                    replyToMessageId: MId

                );
                return;
            }
            else
            {
                await _botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Something went wrong:)");
                return;
            }
        }
    }
}
