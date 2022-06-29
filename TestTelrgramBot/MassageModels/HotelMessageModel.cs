using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;

namespace TestTelrgramBot
{
    public class HotelMessageModel
    {
        public ITelegramBotClient botClient { get; set; }
        public CancellationToken cancellationToken { get; set; }
        public Message message { get; set; }
        public int mId { get; set; }
        public int hotelMId { get; set; }


        public string url { get; set; }
        public string name { get; set; }
        public string bathrooms { get; set; }
        public int bedrooms { get; set; }
        public int beds { get; set; }
        public string[] images { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public int person { get; set; }
        public float rating { get; set; }
        public string address { get; set; }
        public int rate { get; set; }
        public int total { get; set; }
        public string title { get; set; }
        public string amount { get; set; }
        public string[] previewAmenities { get; set; }
        public int numb { get; set; }





        public HotelMessageModel(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message, int numb)
        {
            this.botClient = botClient;
            this.cancellationToken = cancellationToken;
            this.message = message;
            this.numb = numb;
        }
        public async Task<int> GetHotelMessageModel(Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new
            (
               new[]
               {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "More photo 📷", callbackData: $"HotelMorePhoto {numb.ToString()}"),
                        InlineKeyboardButton.WithUrl( "Learn more 🔎",url)
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Show on the map 🗺", callbackData: $"HotelShowOnTheMap {numb.ToString()}"),
                        InlineKeyboardButton.WithCallbackData(text: "Places nearby 📍", callbackData: $"HotelNearbyPlace {numb.ToString()}")
                    },
               }
            );
            int a = 0;
            Message t = await botClient.SendPhotoAsync
                (
                    message.Chat.Id,
                    photo: images[numb],
                    caption: $"{name.ToUpper()}\n" +
                    $"Address: {address}" +
                    $"Bathrooms: {bathrooms}\n" +
                    $"Bedrooms: {bedrooms}\n" +
                    $"Beds: {beds}\n" +
                    $"Rating: {rating}\n" +
                    $"ReviewAmenities: \n{string.Join(", ", previewAmenities)}\n " +
                    $"Total price: {total} $",
                    replyMarkup: inlineKeyboard,
                    cancellationToken: cancellationToken,
                    replyToMessageId: mId
                );
            a = t.MessageId;
            hotelMId = a;

            return a;
        }



        public async Task HandlerCallbackQueryHotel(CallbackQuery callbackQuery, Message message)
        {
            if (callbackQuery.Data.StartsWith("HotelShowOnTheMap"))
            {

                double la = Convert.ToDouble(lat.Replace('.', ','));
                double lo = Convert.ToDouble(lng.Replace('.', ','));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("------------");
                Console.WriteLine(la);
                Console.WriteLine(lo);
                Console.WriteLine();
                Console.WriteLine(Convert.ToDouble(lng));
                Console.WriteLine("------------");
                Console.ResetColor();

                await botClient.SendVenueAsync
                (
                    chatId: message.Chat.Id,
                    latitude: Convert.ToDouble(lat),
                    longitude: Convert.ToDouble(lng),
                    title: name.ToUpper(),
                    address: name,
                    cancellationToken: cancellationToken,
                   replyToMessageId: hotelMId
                );

                return;

            }
            else if (callbackQuery.Data.StartsWith("HotelMorePhoto"))
            {
                Console.WriteLine("HotelMorePhoto");
                await botClient.SendMediaGroupAsync
                (
                   callbackQuery.Message.Chat.Id,
                   media: new IAlbumInputMedia[]
                   {
                       new InputMediaPhoto(images[0]),
                       new InputMediaPhoto(images[1]),
                       new InputMediaPhoto(images[2])
                   },
                   replyToMessageId: hotelMId

                );

                return;
            }
            else if (callbackQuery.Data.StartsWith("HotelNearbyPlace"))
            {
                PlacesNearbyInfoMassegeModel.Long = Convert.ToDouble(lng.Replace('.', ','));
                PlacesNearbyInfoMassegeModel.Lat = Convert.ToDouble(lat.Replace('.', ','));

                InlineKeyboardMarkup inlineKeyboard = new
                (
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Park", callbackData: $"InfoPark"),
                        InlineKeyboardButton.WithCallbackData(text: "Cafe", callbackData: $"InfoCafe"),
                        InlineKeyboardButton.WithCallbackData(text: "Library", callbackData: $"InfoLibrary")

                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Cinema", callbackData: $"InfoCinema"),
                        InlineKeyboardButton.WithCallbackData(text: "Bar", callbackData: $"InfoBar"),
                        InlineKeyboardButton.WithCallbackData(text: "Museum", callbackData: $"InfoMuseum")

                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Restaurant", callbackData: $"InfoRestaurant"),
                        InlineKeyboardButton.WithCallbackData(text: "Parking", callbackData: $"InfoParking")

                    }
                }
                );
                await botClient.SendTextMessageAsync(callbackQuery.Message.Chat, "Choose what you want to find", replyMarkup: inlineKeyboard,
                   replyToMessageId: hotelMId);
                return;
            }
            else
            {
                await botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id, photo: "https://www.meme-arsenal.com/memes/fe290e7bfcc353a46b5ce2e36cb30290.jpg");
                return;
            }
            return;
        }



    }
}
