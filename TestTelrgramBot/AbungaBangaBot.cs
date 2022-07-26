using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Extensions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Extensions.Polling;
using Telegram.Bot.Exceptions;
using System.Text.RegularExpressions;
using TestTelrgramBot.Clients;
using TestTelrgramBot.Models;
using TestTelrgramBot.Constant;

namespace TestTelrgramBot
{
    class AbungaBangaBot
    {
        ReceiverOptions receiverOptions = new ReceiverOptions { AllowedUpdates = { } };
        TelegramBotClient botClient = new TelegramBotClient(Constants.BotKey);
        CancellationToken cancellationToken = new CancellationToken();
        public List<PlacesNearbyInfoMassegeModel> placeNearlyMessages { get; set; }
        public List<HotelMessageModel> hotelMessageModels { get; set; }
        public CityMessageModel cityMessageModel { get; set; }
        private ApiHandler apiHandler { get; set; }
        private string[] userRoutes { get; set; }
        private bool reviewText = false;

        public async Task Start()
        {
            botClient.StartReceiving(HandlerUpdateAsync, HandlerError, receiverOptions, cancellationToken);
            var botMe = await botClient.GetMeAsync();
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
            if (update.Type == UpdateType.CallbackQuery && apiHandler != null)
            {
                //calling different methods to handle pressed buttons depending on callbackData
                if (update.CallbackQuery.Data.StartsWith("Hotel"))
                {
                    Console.WriteLine("HandlerCallbackQueryHotel");
                    ApiHandler.n = int.Parse(update.CallbackQuery.Data.Split(' ')[1]);
                    await apiHandler.hotelMessageModels[ApiHandler.n].HandlerCallbackQueryHotel(update.CallbackQuery, update.CallbackQuery.Message);
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("City"))
                {
                    Console.WriteLine("HandlerCallbackQueryCity");
                    await apiHandler.cityMessageModel.HandlerCallbackQueryCity(update.CallbackQuery);
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("Info"))
                {
                    Console.WriteLine("HandlerCallbackQueryInfo");
                    string type = update.CallbackQuery.Data.Replace("Info", "").ToLower();
                    placeNearlyMessages = await apiHandler.PlacesNearbyMessage(apiHandler.hotelMessageModels[ApiHandler.n].lat, apiHandler.hotelMessageModels[ApiHandler.n].lng, type);
                    if (placeNearlyMessages == null || placeNearlyMessages.Count == 0)
                    {
                        await botClient.SendTextMessageAsync(update.CallbackQuery.Message.From.Id, "An error has occurred");
                        return;
                    }
                    else
                    {
                        int count = placeNearlyMessages.Count > 6 ? 6 : placeNearlyMessages.Count - 1;
                        for (int i = 0; i < count; i++)
                        {
                            if (placeNearlyMessages[i].numb == i)
                            {
                                placeNearlyMessages[i].GetPlacesNearbyModel(update.CallbackQuery.Message);
                                System.Threading.Thread.Sleep(44);
                            }
                        }
                    }
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("Place"))
                {
                    int n = int.Parse(update.CallbackQuery.Data.Split(' ')[1]);
                    await placeNearlyMessages[n].HandlerCallbackQueryInfo(update.CallbackQuery, botClient);
                    return;
                }
                else if (update.CallbackQuery.Data == "SaveRoute")
                {
                    var route = await new DatabaseClient().AddItem(
                        apiHandler.userId,
                        apiHandler.city, PlacesNearbyInfoMassegeModel.routeUrl);
                    Console.WriteLine(route);
                    Console.WriteLine(update.CallbackQuery.From.Username + " saved the route");
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Your route has been saved");
                    return;
                }
                else if (update.CallbackQuery.Data == "Delete")
                {
                    await new DatabaseClient().DeleteAllRoutes(update.CallbackQuery.Message.Chat.Id.ToString(), apiHandler.city);
                    var yur = await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "Route deleted successfully");
                    return;
                }
                else if (update.CallbackQuery.Data.StartsWith("delete"))
                {
                    string[] info = update.CallbackQuery.Data.Split('.');
                    string route = userRoutes[int.Parse(info[3])];

                    var delRoute = await new DatabaseClient().DeleteItem(info[1], info[2], route);

                    Console.WriteLine(update.CallbackQuery.From.Username + " deleted route");
                    await botClient.SendTextMessageAsync(update.CallbackQuery.Message.Chat.Id, "The route has been deleted");
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
            {
                //Console.WriteLine("-----------------");
                //Console.ForegroundColor = ConsoleColor.DarkYellow;
                //Console.WriteLine(message.From.Username);
                //Console.WriteLine(message.From.Id);
                //Console.WriteLine(message.Text);
                //Console.ResetColor();
                //Console.WriteLine("-----------------");
            }
            if (reviewText)
            {
                string Text = $"Пользователь {message.From.Username} отправил отзыв:\n {message.Text}";
                await botClient.SendTextMessageAsync(783450273, Text);
                reviewText = false;
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
                if (apiHandler != null)
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

                    await botClient.SendTextMessageAsync(message.Chat.Id, $"Are you sure you want to delete routes in {apiHandler.city} ?", replyMarkup: inlineKeyboard);
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
                if (apiHandler != null)
                {
                    var te = await new DatabaseClient().GetFromDb(message.From.Id.ToString(), apiHandler.city);
                    userRoutes = te.NewValue;
                    if (te != null && te.NewValue != null && te.NewValue.Length > 0)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();
                        for (int i = 0; i < te.NewValue.Length; i++)
                        {
                            data.Add($"Delete {i + 1} route", $"delete.{message.From.Id}.{apiHandler.city}.{i}");
                        }

                        InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardCallBackData(data);

                        await botClient.SendTextMessageAsync(message.Chat.Id, $"My routes in {apiHandler.city}", replyMarkup: inlineKeyboardMarkup);

                        Console.WriteLine(message.From.Username + " смотрит свои маршруты");
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"No saved routes in {apiHandler.city}");
                        return;
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "Enter the city");
                    return;
                }
            }
            else if (message.Text == "Do you need help?" || message.Text == "/help")
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
                if (apiHandler != null && apiHandler.city != null && apiHandler.city.Length > 0)
                {
                    await apiHandler.weatherMessageModel.GetWeatherMessageModel(message);
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
            else if (message.Text == "/feedback")
            {
                await botClient.SendTextMessageAsync(message.Chat.Id, "Your feedback: ");
                reviewText = true;
                return;
            }
            else if (message.Text == "View my routes" || message.Text == "/routes")
            {
                if (apiHandler != null)
                {
                    var te = await new DatabaseClient().GetFromDb(message.From.Id.ToString(), apiHandler.city);

                    if (te != null && te.NewValue != null && te.NewValue.Length > 0)
                    {
                        Dictionary<string, string> data = new Dictionary<string, string>();

                        for (int i = 0; i < te.NewValue.Length; i++)
                        {
                            data.Add($"{i + 1} route", te.NewValue[i]);
                        }
                        InlineKeyboardMarkup inlineKeyboardMarkup = GetInlineKeyboardUrl(data);

                        await botClient.SendTextMessageAsync(message.Chat.Id, $"My routes in {apiHandler.city}", replyMarkup: inlineKeyboardMarkup);

                        Console.WriteLine(message.From.Username + " looks at his routes");
                        return;
                    }
                    else
                    {
                        await botClient.SendTextMessageAsync(message.Chat.Id, $"No saved routes in {apiHandler.city}");
                        return;
                    }
                }
                else
                {
                    await botClient.SendTextMessageAsync(message.Chat.Id, "<em>Enter the city</em>", ParseMode.Html);
                    return;
                }

            }
            else if (!message.Text.StartsWith("/") && !message.Text.StartsWith("2") && message.Text != null)
            {
                string city = "";

                if (!message.Text.StartsWith("2") && !message.Text.StartsWith("/"))
                    city = message.Text;

                apiHandler = await CheckTheEnteredData(message);

                if (apiHandler != null)
                {
                    if (apiHandler.cityMessageModel != null)
                    {
                        apiHandler.apiHandlerMessageId = await apiHandler.cityMessageModel.GetCityMessageModel(message);

                        apiHandler.cityMessageModel.weatherMessageModel.mId = apiHandler.apiHandlerMessageId;
                        apiHandler.cityMessageModel.mId = apiHandler.apiHandlerMessageId;
                        apiHandler.userId = message.From.Id.ToString();
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

                }
            }
            else if (message.Text.StartsWith("202"))
            {
                string date = "";

                foreach (var day in message.Text.Split(','))
                {
                    date += day.Trim();
                }
                string reg = @"2022-[0-1][0-9]-[0-3][0-9],2022-[0-1][0-9]-[0-3][0-9],[1-9]";
                Regex regex = new Regex(reg);
                Console.WriteLine(apiHandler != null && Regex.IsMatch(message.Text, reg));
                Console.WriteLine($"{apiHandler != null} ||||||||  {Regex.IsMatch(message.Text, reg)}");
                if (apiHandler != null && Regex.IsMatch(message.Text, reg))
                {
                    Console.WriteLine("I am here 1");
                    apiHandler.GetHotelAsync(message.Text);
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
                return;
            }
            else
            {
                await botClient.SendStickerAsync(message.Chat.Id, "https://chpic.su/_data/stickers/j/Jjaba_Sticker/Jjaba_Sticker_007.webp");
                await botClient.SendTextMessageAsync(message.Chat.Id, $"Invalid data entered!");
                return;
            }

        }

        private static InlineKeyboardMarkup GetInlineKeyboardUrl(Dictionary<string, string> buttonsData)
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
        private static InlineKeyboardMarkup GetInlineKeyboardCallBackData(Dictionary<string, string> buttonsData)
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
            Console.WriteLine($"An error occurred while processing a button click!\n{callbackQuery.Data} - is not registered") ;
            return;
        }
        private async Task<ApiHandler> CheckTheEnteredData(Message message)
        {
            //a method that checks the correctness of the entered data (city name).
            //If successful, returns an instance of the class ApiHandler,
            //in which the connection with the api and the processing of information takes place
            ApiHandler handler = new ApiHandler(botClient, cancellationToken, message, message.Text);

            string[] name = message.Text.Split(' ');

            foreach (string name2 in name)
            {
                if (name2.ToLower() == "city" && handler != null)
                {
                    return handler;
                }
            }
            if (handler != null && handler.CorrectlyEnteredCity(message.Text))
            {
                return handler;
            }
            else
            {
                return null;
            }
        }

    }
}






