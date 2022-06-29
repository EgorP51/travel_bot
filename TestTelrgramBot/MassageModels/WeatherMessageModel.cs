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
    public class WeatherMessageModel
    {

        public ITelegramBotClient botClient { get; set; }
        public CancellationToken cancellationToken { get; set; }
        public Message message { get; set; }
        public string forecasts { get; set; }
        public int mId { get; set; }



        public WeatherMessageModel(ITelegramBotClient botClient, CancellationToken cancellationToken, Message message)
        {
            this.botClient = botClient;
            this.cancellationToken = cancellationToken;
            this.message = message;
        }
        public async Task GetWeatherMessageModel(Message message)
        {
            await botClient.SendTextMessageAsync(message.Chat.Id, forecasts, replyToMessageId: mId);

            return;
        }
    }
}
