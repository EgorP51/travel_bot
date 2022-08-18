using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;


namespace TestTelrgramBot
{
    public class HotelMessageModel
    {
        private ITelegramBotClient _botClient;
        private CancellationToken _cancellationToken;
        private int _hotelMId = 0;
        public int MId { get; set; }
        public string Uri { get; set; }
        public string Name { get; set; }
        public string Bathrooms { get; set; }
        public int Bedrooms { get; set; }
        public int Beds { get; set; }
        public string[] Images { get; set; }
        public string Lat { get; set; }
        public string Lng { get; set; }
        public float? Rating { get; set; }
        public int Total { get; set; }
        public string[] PreviewAmenities { get; set; }
        public int Numb { get; set; }

        public HotelMessageModel(ITelegramBotClient botClient, CancellationToken cancellationToken,int numb)
        {
            _botClient = botClient;
            _cancellationToken = cancellationToken;
            Numb = numb;
        }
        public async Task<int> GetHotelMessageModelAsync(Message message)
        {
            InlineKeyboardMarkup inlineKeyboard = new
            (
               new[]
               {
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "More photo 📷", callbackData: $"HotelMorePhoto {Numb.ToString()}"),
                        InlineKeyboardButton.WithUrl( "Learn more 🔎",Uri)
                    },
                    new []
                    {
                        InlineKeyboardButton.WithCallbackData(text: "Show on the map 🗺", callbackData: $"HotelShowOnTheMap {Numb.ToString()}"),
                        InlineKeyboardButton.WithCallbackData(text: "Places nearby 📍", callbackData: $"HotelNearbyPlace {Numb.ToString()}")
                    },
               }
            );
            try
            {
                Message t = await _botClient.SendPhotoAsync
                    (
                        message.Chat.Id,
                        photo: Images[Numb],
                        caption: $"{Name.ToUpper()}\n" +
                        $"Bathrooms: {Bathrooms}\n" +
                        $"Bedrooms: {Bedrooms}\n" +
                        $"Beds: {Beds}\n" +
                        $"Rating: {Rating}\n" +
                        $"ReviewAmenities: \n{string.Join(", ", PreviewAmenities)}\n " +
                        $"Total price: {Total} $",
                        replyMarkup: inlineKeyboard,
                        cancellationToken: _cancellationToken,
                        replyToMessageId: MId
                    );
                _hotelMId = t.MessageId;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return _hotelMId;
        }
        public async Task HotelHandlerCallbackQueryAsync(CallbackQuery callbackQuery, Message message)
        {
            if (callbackQuery.Data.StartsWith("HotelShowOnTheMap"))
            {
                await _botClient.SendVenueAsync
                (
                    chatId: message.Chat.Id,
                    latitude: Convert.ToDouble(Lat),
                    longitude: Convert.ToDouble(Lng),
                    title: Name.ToUpper(),
                    address: Name,
                    cancellationToken: _cancellationToken,
                   replyToMessageId: _hotelMId
                );
                return;
            }
            else if (callbackQuery.Data.StartsWith("HotelMorePhoto"))
            {
                await _botClient.SendMediaGroupAsync
                (
                   callbackQuery.Message.Chat.Id,
                   media: new IAlbumInputMedia[]
                   {
                       new InputMediaPhoto(Images[0]),
                       new InputMediaPhoto(Images[1]),
                       new InputMediaPhoto(Images[2])
                   },
                   replyToMessageId: _hotelMId

                );
                return;
            }
            else if (callbackQuery.Data.StartsWith("HotelNearbyPlace"))
            {
                PlacesNearbyInfoMassegeModel.Long = Convert.ToDouble(Lng);
                PlacesNearbyInfoMassegeModel.Lat = Convert.ToDouble(Lat);

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
                await _botClient.SendTextMessageAsync(
                    chatId: callbackQuery.Message.Chat, 
                    text: "Choose what you want to find", 
                    replyMarkup: inlineKeyboard,
                    replyToMessageId: _hotelMId);
                return;
            }
            else
            {
                await _botClient.SendPhotoAsync(callbackQuery.Message.Chat.Id, photo: "https://www.meme-arsenal.com/memes/fe290e7bfcc353a46b5ce2e36cb30290.jpg");
                return;
            }
        }
    }
}
