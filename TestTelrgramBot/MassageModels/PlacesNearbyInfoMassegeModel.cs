using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TestTelrgramBot
{
    public class PlacesNearbyInfoMassegeModel
    {
        private ITelegramBotClient _botClient;
        private CancellationToken _cancellationToken;
        private Message _message;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone_number { get; set; }
        public int Numb { get; set; } = 0;
        public static double Lat { get; set; }
        public static double Long { get; set; }
        public static string? RouteUrl { get; set; }


        public PlacesNearbyInfoMassegeModel(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)
        {
            _message = message;
            _botClient = botClient;
            _cancellationToken = cancellationToken;
            _message = message;
        }

        public async Task GetPlacesModelAsync(Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new
            (
               new[]
               {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Show on the map", callbackData: $"PlaceMap {Numb}"),
                        InlineKeyboardButton.WithCallbackData(text: "Build a route", callbackData: $"PlaceRoute {Numb}")
                    }
               }
            );
            await _botClient.SendTextMessageAsync
            (
                chatId: message.Chat.Id,
                text: $"{Name}\n{Address}\n{Phone_number}",
                replyMarkup: inlineKeyboard
            );
        }
        public async Task InfoHandlerCallbackQueryAsync(CallbackQuery callbackQuery, ITelegramBotClient botClient)
        {
            if (callbackQuery.Data.StartsWith("PlaceMap"))
            {
                await botClient.SendVenueAsync
                   (
                       chatId: _message.Chat.Id,
                       latitude: Latitude,
                       longitude: Longitude,
                       title: Name,
                       address: Address,
                       cancellationToken: _cancellationToken
                   );
                return;
            }
            else
            if (callbackQuery.Data.StartsWith("PlaceRoute"))
            {
                RouteUrl = $"https://www.google.com/maps/dir/?api=1&origin=" +
                    $"{Latitude.ToString().Replace(',', '.')},{Longitude.ToString().Replace(',', '.')}&destination=" +
                    $"{Lat.ToString().Replace(',', '.')},{Long.ToString().Replace(',', '.')}&travelmode=walking";

                InlineKeyboardMarkup inlineKeyboard = new
              (
                new[]
                {
                    new []
                    {
                        InlineKeyboardButton.WithUrl("Your route is ready",RouteUrl),
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData("Save route 🟢","SaveRoute"),
                    }
                }

              );
                await botClient.SendStickerAsync(_message.Chat.Id, "https://tgram.ru/wiki/stickers/img/JohnnyDepp_videopack/gif/10.gif", replyMarkup: inlineKeyboard);
                return;
            }
        }
    }
}
