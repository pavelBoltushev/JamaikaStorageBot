using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

class CommandHandler
{
    private RequestManager _requestManager;
    private AccountManager _accountManager;
    private Terminal _terminal;

    private Dictionary<string, Command> _commands;

    public void Set(RequestManager requestManager, AccountManager accountManager, Terminal terminal)
    {
        _requestManager = requestManager;
        _accountManager = accountManager;
        _terminal = terminal;
        _commands = FillCommands();
    }

    public async Task HandleMessage(Message message)
    {
        long senderId = message.From.Id;
        string messageText = message.Text;
        int incomingMessageId = message.Id;

        if (_requestManager.IsTextAwaiting(senderId, out int requestId))        
            await HandleTextEnter(senderId, messageText, incomingMessageId, requestId);        
        else if (messageText == "/start")
            await HandleStart(senderId, incomingMessageId);
        else        
            await HandleCommand(senderId, messageText, incomingMessageId);        
    }

    private async Task HandleTextEnter(long senderId, string messageText, int incomingMessageId, int requestId)
    {
        await _terminal.DeleteMessage(senderId, incomingMessageId);
        await _requestManager.EnterText(requestId, messageText);
    }

    private async Task HandleStart(long senderId, int incomingMessageId)
    {
        if (_accountManager.TryGetAccount(senderId, out var account))        
            await _terminal.SendMessage(senderId, account.ToString(), AdmissionMarkup(account));
        else
        {
            _accountManager.AddAccount(senderId);
            await _terminal.SendMessage(_accountManager.AdminId, senderId.ToString());
            await _terminal.SendForwardedMessage(_accountManager.AdminId, senderId, incomingMessageId);
        }
    }

    private async Task HandleCommand(long senderId, string messageText, int incomingMessageId)
    {
        if (_commands.TryGetValue(messageText, out var command) &&
            _accountManager.TryGetAccount(senderId, out var account) &&
            account.Accesses.Contains(command.Access))
        {            
            await command.Process(senderId, incomingMessageId);
        }
    }

    private Dictionary<string, Command> FillCommands()
    {
        return
            new()
            {
                ["📦 СНЯТЬ"] = new(_requestManager.Init<RemoveBoxesRequest>, Access.Администратор),
                ["📦 ПРИЁМКА"] = new(_requestManager.Init<ReceptionAdmissionRequest>, Access.Администратор),
                ["📦 СТЕЛЛАЖИ"] = new(_requestManager.Init<StillagesAdmissionRequest>, Access.Администратор),
                ["📦 СХРОН"] = new(_requestManager.Init<DepoAdmissionRequest>, Access.Администратор),
                ["📈 ОСТАТКИ"] = new(_requestManager.Init<GetStatisticsRequest>, Access.Администратор),
                ["📦 ОБЗОР"] = new(_requestManager.Init<ReviewBoxesRequest>, Access.Администратор),
                ["🚚 СТУДИИ"] = new(_requestManager.Init<StudioShipmentRequest>, Access.Администратор),
                ["🚚 ФИЛИНСКИЙ"] = new(_requestManager.Init<FilinskiyShipmentRequest>, Access.Администратор),
                ["👤 ПОЛЬЗОВАТЕЛИ"] = new(_requestManager.Init<AccountsAdmissionRequest>, Access.Администратор)
                //["🏭 ЗАКАЗЫ"] = new(_requestManager.Init<OrderTrackingRequest>, Access.Администратор)
            };
    }

    private ReplyKeyboardMarkup AdmissionMarkup(Account account)
    {
        var buttonsList = new List<List<KeyboardButton>>();

        foreach (var access in account.Accesses)        
            buttonsList.AddRange(GetButtons(access));        

        var markup = new ReplyKeyboardMarkup(buttonsList);
        markup.ResizeKeyboard = true;
        return markup;
    }    

    private List<List<KeyboardButton>> GetButtons(Access access)
    {
        var buttonsList = new List<List<KeyboardButton>>();
        var commandKeys = _commands.Where(command => command.Value.Access == access)
                                    .Select(command => command.Key)
                                    .ToList();

        for (int i = 0; i < commandKeys.Count; i++)
        {
            var buttonLine = new List<KeyboardButton>();
            buttonLine.Add(new KeyboardButton(commandKeys[i]));
            i++;

            if(i < commandKeys.Count)
                buttonLine.Add(new KeyboardButton(commandKeys[i]));

            buttonsList.Add(buttonLine);
        }

        return buttonsList;        
    }
}