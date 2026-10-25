using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

abstract class Request
{
    private long _senderId;
    private int _messageId;
    private int _commandMessageId;
    private string _messageText;
    private string _serializedMarkup;
    private ScreenUnit _screenUnit;
    private Terminal _terminal;

    protected AccountManager _accountManager;
    protected Storage _storage;
    protected BoxStatsSlidePack _boxStatsSlidePack;
    protected SupplyStatsSlidePack _supplyStatsSlidePack;
    protected SizeSlidePack _sizeSlidePack;
    protected DigitKeyboardSlidePack _digitKeyboardSlidePack;
    protected AdresSlidePack _adresSlidePack;
    protected CoreStatsSlidePack _coreStatsSlidePack;

    protected Request()
    {
        Id = MessageUnitIdCounter.GetId();
        SaveLoader.SaveMessageUnitId();
        _supplyStatsSlidePack = new SupplyStatsSlidePack(Id);
        _sizeSlidePack = new SizeSlidePack(Id);
        _boxStatsSlidePack = new BoxStatsSlidePack(Id);
        _digitKeyboardSlidePack = new DigitKeyboardSlidePack(Id);
        _adresSlidePack = new AdresSlidePack(Id);
        _coreStatsSlidePack = new CoreStatsSlidePack(Id);
    }

    public int Id { get; init; }
    public bool TextAwaiting { get; set; }
    public bool MatchSenderId(long senderId) => _senderId == senderId;

    public event Action<int> RequestFinished;
    protected bool NullScreenUnit => _screenUnit == null;

    public abstract Task InitRequest();
    public abstract Task Execute(bool processing, int slideId = 0);
    public abstract Task EnterText(string text);

    public void Set(long senderId, int commandMessageId, Terminal terminal, Storage storage, AccountManager accountManager)
    {
        _senderId = senderId;
        _commandMessageId = commandMessageId;
        _terminal = terminal;
        _storage = storage;
        _accountManager = accountManager;
    }

    public async Task ProcessSlide(int slideId)
    {
        await Execute(true, slideId);
    }

    public async Task ProcessStage()
    {
        await Execute(false);
    }    

    protected async Task ProcessBoxStatsSlide(int slideId, Box box)
    {
        if (slideId < 0)
        {
            slideId *= -1;
            _boxStatsSlidePack[slideId].ProcessBox(box);
            _messageText = _boxStatsSlidePack[slideId].ProcessText(_messageText);
            await ProcessStage();
        }
        else
        {
            _boxStatsSlidePack[slideId].ProcessBox(box);
            await ChangeMessage(_boxStatsSlidePack[slideId].ProcessText(_messageText),
                _boxStatsSlidePack[slideId].Markup());            
        }
    }

    protected async Task ProcessSupplyStatsSlide(int slideId, Box box)
    {
        if (slideId < 0)
        {
            slideId *= -1;
            _supplyStatsSlidePack[slideId].ProcessBox(box);
            _messageText = _supplyStatsSlidePack[slideId].ProcessText(_messageText);
            await ProcessStage();
        }
        else
        {
            _supplyStatsSlidePack[slideId].ProcessBox(box);
            await ChangeMessage(_supplyStatsSlidePack[slideId].ProcessText(_messageText),
                _supplyStatsSlidePack[slideId].Markup());
        }
    }

    protected async Task ProcessCoreStatsSlide(int slideId, Box box)
    {
        if (slideId < 0)
        {
            slideId *= -1;
            _coreStatsSlidePack[slideId].ProcessBox(box);
            _messageText = _coreStatsSlidePack[slideId].ProcessText(_messageText);
            await ProcessStage();
        }
        else
        {
            _coreStatsSlidePack[slideId].ProcessBox(box);
            await ChangeMessage(_coreStatsSlidePack[slideId].ProcessText(_messageText),
                _coreStatsSlidePack[slideId].Markup());
        }
    }

    protected async Task ProcessSizeSlide(int slideId, Box box)
    {
        if (slideId < 0)
        {
            slideId *= -1;
            _sizeSlidePack[slideId].ProcessBox(box);
            _messageText = _sizeSlidePack[slideId].ProcessText(_messageText);
            await ProcessStage();
        }
        else
        {
            _sizeSlidePack[slideId].ProcessBox(box);
            await ChangeMessage(_sizeSlidePack[slideId].ProcessText(_messageText),
                _sizeSlidePack[slideId].Markup());
        }
    }

    protected async Task ProcessAdresSlide(int slideId, Adres adress)
    {
        if (slideId < 0)
        {
            await ProcessStage();
        }
        else
        {
            _adresSlidePack[slideId].ProcessAdress(adress);
            await ChangeMessage(_adresSlidePack[slideId].ProcessText(_messageText),
                _adresSlidePack[slideId].Markup()); 
        }
    }

    protected async Task<int> ProcessDigitKeyboardSlide(int slideId, int currentInt)
    {
        if (slideId < 0)
        {
            await ProcessStage();
            return currentInt;
        }
        else
        {
            currentInt = _digitKeyboardSlidePack[slideId].ProcessInt(currentInt);
            await ChangeMessage(_digitKeyboardSlidePack[slideId].ProcessText(_messageText),
                _digitKeyboardSlidePack[slideId].Markup());   
            return currentInt;
        }
    }

    protected async Task CreateMessage(string text, InlineKeyboardMarkup markup)
    {
        _messageText = text;
        _serializedMarkup = JsonConvert.SerializeObject(markup);
        var message = await _terminal.CreateMessage(_senderId, _messageText, markup);
        _messageId = message.Id;
    }

    protected async Task ChangeMessage(string text, InlineKeyboardMarkup markup)
    {
        if (_messageText == text && _serializedMarkup == JsonConvert.SerializeObject(markup))
            return;
        
        _messageText = text;
        _serializedMarkup = JsonConvert.SerializeObject(markup);
        await _terminal.ChangeMessage(_senderId, _messageId, _messageText, markup);              
    }

    protected async Task AddLogToMessage(string log, InlineKeyboardMarkup markup)
    {
        _messageText += log;
        _serializedMarkup = JsonConvert.SerializeObject(markup);
        await _terminal.ChangeMessage(_senderId, _messageId, _messageText, markup);
    }

    protected async Task SendReport(Report report, string text)
    {
        var senderIds = _accountManager.GetSenderIds(report);

        for (int i = 0; i < senderIds.Count; i++)
        {
            await _terminal.SendMessage(senderIds[i], text);
        }
    }

    protected async Task<ScreenUnit> CreateScreenUnit(Screen screen, string text)
    {
        _screenUnit = new ScreenUnit();
        _screenUnit.MessageText = text;
        _screenUnit.Screen = screen;
        var senderIds = _accountManager.GetSenderIds(screen);

        for (int i = 0; i < senderIds.Count; i++)
        {
            var message = await _terminal.CreateMessage(senderIds[i], _screenUnit.TextView, null);
            _screenUnit.Ids.Add((senderIds[i], message.Id));
        }   
        
        return _screenUnit;
    }

    protected async Task<ScreenUnit> ReCreateScreenUnit(string text)
    {
        ScreenUnit newScreenUnit = new ScreenUnit();
        newScreenUnit.MessageText = text;
        newScreenUnit.Screen = _screenUnit.Screen;

        for (int i = 0; i < _screenUnit.Ids.Count; i++)
        {
            long currentSenderId = _screenUnit.Ids[i].senderId;
            int currentMessageId = _screenUnit.Ids[i].messageId;            
            await _terminal.DeleteMessage(currentSenderId, currentMessageId);          
            var newMessage = await _terminal.CreateMessage(currentSenderId, newScreenUnit.TextView, null);
            newScreenUnit.Ids.Add((currentSenderId, newMessage.Id));
        }

        _screenUnit = newScreenUnit;
        return _screenUnit;
    }

    protected async Task DeleteScreenUnit()
    {
        for (int i = 0; i < _screenUnit.Ids.Count; i++)
        {
            await _terminal.DeleteMessage(_screenUnit.Ids[i].senderId,
                                            _screenUnit.Ids[i].messageId);
        }    

        _screenUnit = null;
    }

    protected void SetScreenUnit(ScreenUnit screenUnit)
    {
        _screenUnit = screenUnit;
    }

    protected async Task ChangeScreenUnit(string text)
    {
        if (_screenUnit.MessageText == text)
            return;

        _screenUnit.MessageText = text;

        for (int i = 0; i < _screenUnit.Ids.Count; i++)
        {
            await _terminal.ChangeMessage(_screenUnit.Ids[i].senderId, _screenUnit.Ids[i].messageId, _screenUnit.TextView);
        }
    }
    
    protected (long, int) GetIds()
    {
        return (_senderId, _messageId);
    }

    protected async Task CloseRequest()
    {
        RequestFinished?.Invoke(Id);
        await _terminal.DeleteMessage(_senderId, _commandMessageId);
        await _terminal.DeleteMessage(_senderId, _messageId);
    }

    protected async Task CloseRequest(string closingMessageText)
    {
        RequestFinished?.Invoke(Id);
        await _terminal.ChangeMessage(_senderId, _messageId, closingMessageText);
    }
}