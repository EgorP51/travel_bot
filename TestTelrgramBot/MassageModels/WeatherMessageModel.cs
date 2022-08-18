using Telegram.Bot;
using Telegram.Bot.Types;

namespace TestTelrgramBot
{
    public class WeatherMessageModel
    {
        private ITelegramBotClient _botClient;
        public string forecasts { get; set; }
        public int mId { get; set; }

        public WeatherMessageModel(ITelegramBotClient botClient)
        {
            _botClient = botClient;
        }
        public async Task GetWeatherMessageModel(Message message)
        {
            await _botClient.SendTextMessageAsync(message.Chat.Id, forecasts, replyToMessageId: mId);
        }
    }
}
