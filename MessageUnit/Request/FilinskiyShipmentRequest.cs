using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class FilinskiyShipmentRequest : Request
{
    private List<Order> _orderShipment = new ();
    private Order _newOrder = new ();

    private int _orderIndex;
    private bool _dataChanged;
    private int _stage;

    private Order CurrentOrder => _orderShipment[_orderIndex];

    public override async Task InitRequest()
    {
        _orderShipment = _storage.GetOrderShipment();
        await CreateMessage(GetShipmentText(), OperationMarkup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessOperationSlide(slideId);
                }
                break;

            case 1:
                if (processing)
                {
                    _newOrder.Number = await ProcessDigitKeyboardSlide(slideId, _newOrder.Number);
                }
                else
                {
                    await AddLogToMessage("\n>Кол-во мест: ", _digitKeyboardSlidePack[0].Markup());
                    _stage = 2;
                }
                break;

            case 2:
                if (processing)
                {
                    _newOrder.Count = await ProcessDigitKeyboardSlide(slideId, _newOrder.Count);
                }
                else
                {
                    _newOrder.Status = Status.Отгрузка;
                    _orderShipment.Add(_newOrder);
                    _newOrder.ScreenUnit = await CreateScreenUnit(Screen.Филинский, _newOrder.ToString());
                    _orderIndex = _orderShipment.Count - 1;
                    SaveLoader.Save(_orderShipment);
                    await ChangeMessage(GetShipmentText(), OperationMarkup());
                    _stage = 0;
                }
                break;
        }
    }

    public override async Task EnterText(string text)
    {
        CurrentOrder.Sign = text;
        _dataChanged = true;
        await ChangeMessage(GetShipmentText(), OperationMarkup());
        _stage = 0;
        TextAwaiting = false;
    }

    private string GetShipmentText()
    {
        var text = new StringBuilder();
        text.Append($"📺 Филинский  {GeneralCountToString()}\n" +
            $" - - - - - - - - - - - - - - - - - - - - - \n");

        if (_orderShipment.Count == 0)
            text.Append("- отгрузка пуста -");
        else
        {
            for (int i = 0; i < _orderShipment.Count; i++)
            {
                text.Append((i == _orderIndex ? $"{_orderShipment[i].MarkedToString()}\n" : $"{_orderShipment[i].ToString()}\n"));                
            }
        }

        return text.ToString();
    }

    private string GeneralCountToString()
    {
        int boxesCount = 0;
        int packsCount = 0;

        foreach (var order in _orderShipment)
        {
            if (order.Count == 0)
                packsCount++;
            else
                boxesCount += order.Count;
        }

        return $"{(boxesCount != 0 ? $"📦 - {boxesCount}  " : "")}" +
            $"{(packsCount != 0 ? $"🏷 - {packsCount}": "")}";
    }

    private InlineKeyboardMarkup OperationMarkup()
    {
        var buttonsList = new List<List<InlineKeyboardButton>>();

        if (_orderShipment.Count > 1)
            buttonsList.Add(new() { new("⬆️", $"{Id} {1}"), new("⬇️", $"{Id} {2}") });

        if (_orderShipment.Count != 0)
        {           
            switch (CurrentOrder.Status)
            {
                case Status.Отгрузка:
                    buttonsList.Add(new() { new("✅ Отгрузить", $"{Id} {3}") });
                    break;

                case Status.Выполнен:
                    buttonsList.Add(new() { new("<< 🚚", $"{Id} {4}") });
                    break;
            }

            buttonsList.Add(new() { new("💬 Макет", $"{Id} {5}") });
        }

        if (_dataChanged)
            buttonsList.Add(new() { new("💾 Сохранить", $"{Id} {6}") });
        else
        {
            buttonsList.Add(new() { new("➕ ЗАКАЗ", $"{Id} {7}") });
            buttonsList.Add(new() { new("❌ Закрыть", $"{Id} {8}") });
        }

        return new InlineKeyboardMarkup(buttonsList);
    }

    private async Task ProcessOperationSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                _orderIndex = _orderIndex == 0 ? _orderShipment.Count - 1 : --_orderIndex;
                break;

            case 2:
                _orderIndex = _orderIndex == _orderShipment.Count - 1 ? 0 : ++_orderIndex;
                break;

            case 3:
                CurrentOrder.Status = Status.Выполнен;
                _dataChanged = true;
                break;

            case 4:
                CurrentOrder.Status = Status.Отгрузка;
                _dataChanged = true;
                break;

            case 5:
                await ChangeMessage(CurrentOrder.SignAwaitToString(), null);
                TextAwaiting = true;
                return;

            case 6:
                await SaveShipment();
                _orderIndex = 0;
                _dataChanged = false;
                break;

            case 7:
                _newOrder = new Order();
                await ChangeMessage("НОВЫЙ ЗАКАЗ\n>Номер: ", _digitKeyboardSlidePack[0].Markup());
                _stage = 1;
                return;

            case 8:
                await CloseRequest();
                return;
        }

        await ChangeMessage(GetShipmentText(), OperationMarkup());
    }

    private async Task SaveShipment()
    {
        foreach (var order in _orderShipment)
        {
            SetScreenUnit(order.ScreenUnit);            
            await ChangeScreenUnit(order.ToString());
        }

        for (int i = 0; i < _orderShipment.Count; i++)
        {
            if (_orderShipment[i].Status == Status.Выполнен)
            {
                _orderShipment.Remove(_orderShipment[i]);
                i--;
            }
        }

        SaveLoader.Save(_orderShipment);
    }
}