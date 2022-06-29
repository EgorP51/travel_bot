using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestTelrgramBot
{
    public class CityMessageModel
    {
        public ITelegramBotClient botClient { get; set; }
        public CancellationToken cancellationToken { get; set; }
        public List<HotelMessageModel> hotelMessageModels { get; set; }
        public WeatherMessageModel weatherMessageModel { get; set; }
        public Message message { get; set; }
        public string city { get; set; }
        public string mainPhoto { get; set; }
        public string title { get; set; }
        public string body { get; set; }
        public string infoUrl { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public int mId { get; set; }




        public CityMessageModel(string city, ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)

        {
            this.city = city;
            this.botClient = botClient;
            this.cancellationToken = cancellationToken;
        }

        public async Task<int> GetCityMessageModel(Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new
            (
               new[]
               {
                    new []
                    {
                        InlineKeyboardButton.WithUrl(text: "More info ℹ️", infoUrl),
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
            Message mes;
            try
            {
                mes = await botClient.SendPhotoAsync
                (
                    message.Chat.Id,
                    photo: mainPhoto,
                    caption: $"{title}\n{body}",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken
                );
            }
            catch (Exception ex)
            {
                mes = await botClient.SendPhotoAsync
                (
                   message.Chat.Id,
                   photo: "https://www.salonlfc.com/wp-content/uploads/2018/01/image-not-found-scaled-1150x647.png",
                   caption: $"(city ​​photos not found)\n{title}\n{body}",
                   replyMarkup: inlineKeyboard,
                   cancellationToken: cancellationToken
                );
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(">");
            Console.WriteLine($" Main photo city: {mainPhoto}");
            Console.WriteLine(">");
            Console.WriteLine($" info url: {infoUrl}");
            Console.WriteLine(">");
            Console.ResetColor();

            return mes.MessageId;
        }

        public async Task HandlerCallbackQueryCity(CallbackQuery callbackQuery)
        {
            Console.WriteLine(callbackQuery.Data);
            if (callbackQuery.Data == "CityWeather")
            {
                await weatherMessageModel.GetWeatherMessageModel(message);
            }
            else if (callbackQuery.Data == "CityHotels")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
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
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Should help)))", replyMarkup: inlineKeyboard);
                return;
            }
            else if (callbackQuery.Data == "CityShowOnTheMap")
            {
                Console.WriteLine(latitude);
                Console.WriteLine(longitude);
                await botClient.SendVenueAsync
                (
                    chatId: callbackQuery.Message.Chat.Id,
                    latitude: latitude,
                    longitude: longitude,
                    title: city,
                    address: title,
                    cancellationToken: cancellationToken,
                    replyToMessageId: mId

                );
                return;
            }
            else
            {
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, "Что-то пошло не так:)");
                return;
            }
            return;
        }
    }
}
