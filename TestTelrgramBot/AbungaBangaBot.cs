using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using System.Text.RegularExpressions;
using TestTelrgramBot.Clients;
using TestTelrgramBot.Constant;

namespace TestTelrgramBot
{
    class AbungaBangaBot
    {
        private ReceiverOptions _receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        private TelegramBotClient _botClient = new TelegramBotClient(Constants.BotKey);
        private CancellationToken _cancellationToken = new CancellationToken();
        private List<PlacesNearbyInfoMassegeModel>? _placeNearlyMessages;
        private ApiHandler? _apiHandler;
        private string[]? _userRoutes;
        private bool _isFeedbackText = false;

        public async Task Start()
        {
            _botClient.StartReceiving(HandlerUpdateAsync, HandlerError, _receiverOptions, _cancellationToken);
            var botMe = await _botClient.GetMeAsync();
            Console.WriteLine("Bot at work...");
            Console.ReadKey();
        }
        private Task HandlerError(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
        {
            var ErrorMassage = exception switch
            {
                ApiRequestException apiRequestException => $"Damn, there's an error with the API:\n{apiRequestException.ErrorCode}" +
                $"\n{apiRequestException.Message}",
                _ => exception.ToString()
            };
            Console.WriteLine(ErrorMassage);
            return Task.CompletedTask;
        }

        //Work with all updates happens here
        private async Task HandlerUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
        {
            if (update.Type == UpdateType.Message && update.Message?.Text != null)
            {
                //calling a message handler method
                await HandlerMassageAsync(botClient, update.Message);
                return;
            }
            if (update.Type == UpdateType.CallbackQuery && _apiHandler != null)
            {
                //calling different methods to handle pressed buttons depending on callbackData
                if (update.CallbackQuery.Data.StartsWith("Hotel"))
                {
                    ApiHandler.N = int.Parse(update.CallbackQuery.Data.Split(' ')[1]);
                    await _apiHandler.HotelMessageModels[ApiHandler.N].HotelHandlerCallbackQueryAsync(update.CallbackQuery, update.CallbackQuery.Message);
                    return;
                }
                else 
                if (update.CallbackQuery.Data.StartsWith("City"))
                {
                    await _apiHandler.CityMessageModel.CityHandlerCallbackQueryAsync(update.CallbackQuery);
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("Info"))
                {
                    string lat = _apiHandler.HotelMessageModels[ApiHandler.N].Lat;
                    string lng = _apiHandler.HotelMessageModels[ApiHandler.N].Lng;
                    string type = update.CallbackQuery.Data.Replace("Info", "").ToLower();

                    _placeNearlyMessages = await _apiHandler.PlacesNearbyMessageAsync(lat,lng, type);
                    
                    if (_placeNearlyMessages == null || _placeNearlyMessages.Count == 0)
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.From.Id, "An error has occurred");
                        return;
                    }
                    else
                    {
                        int count = _placeNearlyMessages.Count > 6 ? 6 : _placeNearlyMessages.Count;
                        for (int i = 0; i < count; i++)
                        {
                            if (_placeNearlyMessages[i].Numb == i)
                            {
                                _placeNearlyMessages[i].GetPlacesModelAsync(update.CallbackQuery.Message);
                                Thread.Sleep(44);
                            }
                        }
                    }
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("Place"))
                {
                    int placeNumber = int.Parse(update.CallbackQuery.Data.Split(' ')[1]);
                    await _placeNearlyMessages[placeNumber].InfoHandlerCallbackQueryAsync(update.CallbackQuery, botClient);
                    return;
                }
                else if (update.CallbackQuery.Data == "SaveRoute")
                {
                    var route = await new DatabaseClient().AddItemAsync(
                        _apiHandler.UserId,
                        _apiHandler.City, 
                        PlacesNearbyInfoMassegeModel.RouteUrl);

                    if (route != null)
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Your route has been saved");
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Some error!");
                    }
                    return;
                }
                else if (update.CallbackQuery.Data == "Delete")
                {
                    string userId = update.CallbackQuery.Message.Chat.Id.ToString();
                    string city = _apiHandler.City;
                    try
                    {
                        await new DatabaseClient().DeleteAllRoutesAsync(userId,city);
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Route deleted successfully");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Some error");
                    }
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("delete"))
                {
                    string[] info = update.CallbackQuery.Data.Split('.');
                    string route = _userRoutes[int.Parse(info[3])];

                    try
                    {
                        await new DatabaseClient().DeleteItemAsync(info[1], info[2], route);
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "The route has been deleted");
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Some error");
                    }
                    return;
                }
                else
                {
                    await HandlerCallbackQuery(botClient, update.CallbackQuery);
                    return;
                }
            }

        }

        private async Task HandlerMassageAsync(ITelegramBotClient botClient, Message message)
        {
            if (_isFeedbackText)
            {
                string Text = $"Пользователь {message.From.Username} отправил отзыв:\n {message.Text}";
                await botClient.SendTextMessageAsync(783450273, Text);
                _isFeedbackText = false;
                await botClient.SendTextMessageAsync(message.Chat.Id, "Thanks ✨");
                return;
            }
            else
            if (message.Text == "/start")
            {
                ReplyKeyboardMarkup replyKeyboardMarkup =
                new
                (
                    new[]
                    {
                        new KeyboardButton[] { "City, information, hotels", "View my routes"},
                        new KeyboardButton[] { "Do you need help?" }
                    }
                )
                {
                    ResizeKeyboard = true
                };
                await botClient.SendTextMessageAsync(
                chatId: message.Chat.Id, "Hello, this bot is designed to help tourists. " +
                "Choose the city where you would like to go, check the weather, " +
                "find a hotel that you like. You can also immediately check the " +
                "availability of useful places near your hotel. Search for places,\n" +
                " build and add routes. Have a good trip! \n" +
                "If you need help, use the /help command\n" +
                "(Lower your eyes to get started 👇)"
                , replyMarkup: replyKeyboardMarkup);

                return;
            }
            else if (message.Text == "City, information, hotels" || message.Text == "/city")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the city where you would like to go");
                return;
            }
            else if (message.Text == "/deleteall")
            {
                if (_apiHandler != null)
                {
                    InlineKeyboardMarkup inlineKeyboard = new
                    (
                    new[]
                    {
                        new []
                        {
                            InlineKeyboardButton.WithCallbackData("Delete routes 🔴",callbackData: "Delete")
                        }
                    }

                    );

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Are you sure you want to delete routes in {_apiHandler.City} ?", replyMarkup: inlineKeyboard);
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                    "Enter the city where you want to delete route and repeat the command call");
                }
            }
            else if (message.Text == "/deleteone")
            {
                if (_apiHandler != null)
                {
                    var cityInfoFromDb = await new DatabaseClient().GetFromDbAsync(message.From.Id.ToString(), _apiHandler.City);
                    _userRoutes = cityInfoFromDb.NewValue;

                    bool routeListIsEmpty = cityInfoFromDb == null && cityInfoFromDb.NewValue == null && cityInfoFromDb.NewValue.Length == 0;
                    
                    if (!routeListIsEmpty)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        for (int i = 0; i < cityInfoFromDb.NewValue.Length; i++)
                        {
                            data.Add($"Delete {i + 1} route", $"delete.{message.From.Id}.{_apiHandler.City}.{i}");
                        }

                        InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(data);
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"My routes in {_apiHandler.City}", replyMarkup: inlineKeyboardMarkup);
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"No saved routes in {_apiHandler.City}");
                        return;
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the city");
                    return;
                }
            }
            else 
            if (message.Text == "Do you need help?" || message.Text == "/help")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id,
                    "/start getting started with the bot\n" +
                    "/help if you need help\n" +
                    "/routes view your routes\n" +
                    "/weather see the weather\n" +
                    "/feedback leave feedback\n" +
                    "/city city ​​information search\n" +
                    "To add a route, enter the name of the city and select a hotel," +
                    " click on places nearby and select what you need.  You can save " +
                    "the route to this place from the selected hotel");
                return;
            }
            else if (message.Text == "/weather")
            {
                bool weatherInfoIsNull = _apiHandler == null && _apiHandler.City == null && _apiHandler.City.Length == 0;

                if (!weatherInfoIsNull)
                {
                    await _apiHandler.WeatherMessageModel.GetWeatherMessageModel(message);
                    return;
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        "Enter the city where you want to see the " +
                        "weather and repeat the command call");
                    return;
                }
            }
            else 
            if (message.Text == "/feedback")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Your feedback: ");
                _isFeedbackText = true;
                return;
            }
            else 
            if (message.Text == "View my routes" || message.Text == "/routes")
            {
                if (_apiHandler != null)
                {
                    var cityInfoFromDb = await new DatabaseClient().GetFromDbAsync(message.From.Id.ToString(), _apiHandler.City);
                    bool routeListIsEmpty = cityInfoFromDb == null && cityInfoFromDb.NewValue == null && cityInfoFromDb.NewValue.Length == 0;
                    
                    if (!routeListIsEmpty)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        for (int i = 0; i < cityInfoFromDb.NewValue.Length; i++)
                        {
                            data.Add($"{i + 1} route", cityInfoFromDb.NewValue[i]);
                        }

                        InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardUrl(data);
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"My routes in {_apiHandler.City}", replyMarkup: inlineKeyboardMarkup);
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"No saved routes in {_apiHandler.City}");
                        return;
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the city");
                    return;
                }
            }
            else 
            if (!message.Text.StartsWith("/") && !message.Text.StartsWith("2") && message.Text != null)
            {
                _apiHandler = CheckTheEnteredData(message);

                if (_apiHandler != null)
                {
                    if (_apiHandler.CityMessageModel != null)
                    {
                        _apiHandler.ApiHandlerMessageId = await _apiHandler.CityMessageModel.GetCityMessageModelAsync(message);

                        _apiHandler.CityMessageModel.WeatherMessageModel.mId = _apiHandler.ApiHandlerMessageId;
                        _apiHandler.CityMessageModel.MId = _apiHandler.ApiHandlerMessageId;
                        _apiHandler.UserId = message.From.Id.ToString();
                    }
                    else
                    {
                        await botClient.SendStickerAsync(message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"Damn it's wrong, please try again");
                        return;
                    }
                }
                else
                {
                    await botClient.SendStickerAsync(message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                    await botClient.SendTextMessageAsync(message.Chat.Id,
                        $"An error occurred, the city of {message.Text} may " +
                        $"not exist. Try to correct the name or add the \"city\" to the name");
                    return;
                }
            }
            else 
            if (message.Text.StartsWith("202"))
            {
                string reg = @"2022-[0-1][0-9]-[0-3][0-9],2022-[0-1][0-9]-[0-3][0-9],[1-9]";

                if (_apiHandler != null && Regex.IsMatch(message.Text, reg))
                {
                    _apiHandler.GetHotelAsync(message.Text);
                    return;
                }
                else
                {
                    await botClient.SendStickerAsync(message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Damn, there is an error, please try entering the date again\n" +
                        $"Date format: 2022-07-13,2022-07-15,2\n" +
                        $"(Perhaps the city is not entered)");
                    return;
                }
            }
            else
            {
                await botClient.SendStickerAsync(message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Invalid data entered!");
                return;
            }

        }

        private InlineKeyboardMarkup GetInlineKeyboardUrl(Dictionary<string, string> buttonsData)
        {
            //the method accepts a Dictionary<string text, string url>
            // and returns an inline keyboard

            // Rows Count
            int count = buttonsData.Count;

            // List of rows 
            // Every 'List<InlineKeyboardButton>' is row
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(count);

            for (int i = 0; i < count; i++)
            {
                // Add new row with one button capacity
                buttons.Add(new List<InlineKeyboardButton>(1)
                {
                    new InlineKeyboardButton("some text")
                    {
                       Text = buttonsData.Keys.ElementAt(i),
                       Url = buttonsData.Values.ElementAt(i),
                    }
                });
            }
            return new InlineKeyboardMarkup(buttons);
        }
        private InlineKeyboardMarkup GetInlineKeyboardCallBackData(Dictionary<string, string> buttonsData)
        {
            //the method accepts a Dictionary<string text, string callBackData>
            // and returns an inline keyboard

            // Rows Count
            int count = buttonsData.Count;

            // List of rows 
            // Every 'List<InlineKeyboardButton>' is row
            List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>(count);

            for (int i = 0; i < count; i++)
            {
                // Add new row with one button capacity
                buttons.Add(new List<InlineKeyboardButton>(1)
                {
                    new InlineKeyboardButton("some tex")
                    {
                       Text = buttonsData.Keys.ElementAt(i),
                       CallbackData = buttonsData.Values.ElementAt(i),
                    }
                });
            }
            return new InlineKeyboardMarkup(buttons);
        }
        async Task HandlerCallbackQuery(ITelegramBotClient botClient, CallbackQuery callbackQuery)
        {
            //the method is called if the processing of pressing a key is not registered
            await botClient.SendTextMessageAsync(callbackQuery.Message.Chat.Id, text: "An error occurred while processing a button click");
            return;
        }
        private ApiHandler? CheckTheEnteredData(Message message)
        {
            //a method that checks the correctness of the entered data (city name).
            //If successful, returns an instance of the class ApiHandler,
            //in which the connection with the api and the processing of information takes place
            ApiHandler handler = new ApiHandler(_botClient, _cancellationToken, message, message.Text);

            string[] name = message.Text.Split(' ');

            foreach (string name2 in name)
            {
                if (name2.ToLower() == "city" && handler != null)
                {
                    return handler;
                }
            }
            return handler != null && handler.CorrectlyEnteredCity(message.Text) ? handler : null;
        }
    }
}






