using Telegram.Bot;
using Telegram.Bot.Polling;

class BotBuilder
{
    private ITelegramBotClient _bot = new TelegramBotClient("");
    private Terminal _terminal = new Terminal();
    private UpdateHandler _updateHandler = new UpdateHandler();
    private CommandHandler _commandHandler = new CommandHandler();
    private AccountManager _accountManager = new AccountManager();
    private Storage _storage = new Storage();
    private QueryHandler _queryHandler = new QueryHandler();
    private RequestManager _requestManager = new RequestManager();

    public BotBuilder()
    {
        _terminal.Set(_bot);
        _updateHandler.Set(_commandHandler, _queryHandler, _bot);
        _commandHandler.Set(_requestManager, _accountManager, _terminal);       
        _queryHandler.Set(_requestManager);
        _requestManager.Set(_storage, _terminal, _accountManager);

        SaveLoader.Load(_storage, _accountManager);
    }

    public static BotBuilder Build()
    {
        return new BotBuilder();
    }

    public void Run()
    {
        Console.WriteLine("Запущен бот " + _bot.GetMe().Result.FirstName);
        var cts = new CancellationTokenSource();
        var cancellationToken = cts.Token;
        var receiverOptions = new ReceiverOptions();

        _bot.StartReceiving(
            _updateHandler.HandleUpdateAsync,
            _updateHandler.HandleErrorAsync,
            receiverOptions,
            cancellationToken
        );

        Console.ReadKey();
    }
}