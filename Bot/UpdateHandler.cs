using Telegram.Bot;
using Telegram.Bot.Types;

class UpdateHandler
{
    private CommandHandler _commandHandler;
    private QueryHandler _queryHandler;
    private ITelegramBotClient _bot;

    public void Set(CommandHandler commandHandler, QueryHandler queryHandler, ITelegramBotClient bot)
    {
        _commandHandler = commandHandler;
        _queryHandler = queryHandler;
        _bot = bot;
    }

    public async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken cancellationToken)
    {
        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message && update.Message.Text != null)
        {            
            await _commandHandler.HandleMessage(update.Message);
        }

        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery && update.CallbackQuery.Data != null)
        {
            string callbackData = update.CallbackQuery.Data;
            await _queryHandler.HandelQuery(callbackData);
            await _bot.AnswerCallbackQuery(update.CallbackQuery.Id);
        }
    }

    public async Task HandleErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken) 
    {
        Console.WriteLine(exception.ToString());
    }
}