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
using Newtonsoft.Json;

namespace TestTelrgramBot
{
    public class PlacesNearbyInfoMassegeModel
    {
        public ITelegramBotClient botClient { get; set; }
        public CancellationToken cancellationToken { get; set; }
        public Message message { get; set; }
        public string body { get; set; }
        public int mId { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string phone_number { get; set; }
        public string distance { get; set; }
        public int numb { get; set; } = 0;

        public static double Lat { get; set; }
        public static double Long { get; set; }
        public static string routeUrl { get; set; }


        public PlacesNearbyInfoMassegeModel(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)
        {
            this.message = message;
            this.botClient = botClient;
            this.cancellationToken = cancellationToken;
            this.message = message;
        }

        public async Task GetPlacesNearbyModel(Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new
            (
               new[]
               {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Show on the map", callbackData: $"PlaceMap {numb}"),
                        InlineKeyboardButton.WithCallbackData(text: "Build a route", callbackData: $"PlaceRoute {numb}")
                    }
               }
            );
            await botClient.SendTextMessageAsync
            (

                chatId: message.Chat.Id,
                text: $"{name}\n{address}\n{phone_number}",
                replyMarkup: inlineKeyboard
            );
        }

        public async Task HandlerCallbackQueryInfo(CallbackQuery callbackQuery, ITelegramBotClient botClient)
        {
            if (callbackQuery.Data.StartsWith("PlaceMap"))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("-------------");
                Console.WriteLine(latitude);
                Console.WriteLine(longitude);
                Console.WriteLine("-------------");
                Console.ResetColor();
                try
                {
                    await botClient.SendVenueAsync
                    (
                        chatId: message.Chat.Id,
                        latitude: latitude,
                        longitude: longitude,
                        title: name,
                        address: address,
                        cancellationToken: cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    Message t = await botClient.SendTextMessageAsync
                    (
                        message.Chat.Id,
                        text: "An error has occurred",
                        cancellationToken: cancellationToken,
                        replyToMessageId: mId
                    );
                }
                return;
            }
            if (callbackQuery.Data.StartsWith("PlaceRoute"))
            {
                Console.ForegroundColor = ConsoleColor.Red;

                Console.WriteLine(latitude);
                Console.WriteLine(longitude);
                Console.WriteLine("------------");
                Console.WriteLine(Lat);
                Console.WriteLine(Long);
                Console.WriteLine("------------");
                Console.ResetColor();
                routeUrl = $"https://www.google.com/maps/dir/?api=1&origin={latitude.ToString().Replace(',', '.')},{longitude.ToString().Replace(',', '.')}&destination={Lat.ToString().Replace(',', '.')},{Long.ToString().Replace(',', '.')}&travelmode=walking";
                InlineKeyboardMarkup inlineKeyboard = new
              (
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Your route is ready",routeUrl),
                    },
                     new []
                    {
                        InlineKeyboardButton.WithCallbackData("Save route 🟢","SaveRoute"),
                    }
                }

             );


                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine(">");
                Console.WriteLine($" Your route: https://www.google.com/maps/dir/?api=1&origin={latitude.ToString().Replace(',', '.')},{longitude.ToString().Replace(',', '.')}&destination={Lat.ToString().Replace(',', '.')},{Long.ToString().Replace(',', '.')}&travelmode=walking");
                Console.WriteLine(">");

                Console.ResetColor();
                //await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, $" ",replyMarkup: inlineKeyboard);
                await botClient.SendStickerAsync(message.Chat.Id, "https://tgram.ru/wiki/stickers/img/JohnnyDepp_videopack/gif/10.gif", replyMarkup: inlineKeyboard);
                return;
            }
        }
    }
}
