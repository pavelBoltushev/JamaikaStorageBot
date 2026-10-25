using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class DepoAdmissionRequest : Request
{
    private List<DepoCell> _depo = new List<DepoCell>();
    private Box _currentBox = new Box();
    private Adres _newAdress = new Adres();
    private StringBuilder _report = new StringBuilder();

    private int _depoCellIndex;
    private DepoCellSide _currentDepoCellSide; 
    private int _depoCellNumber;
    private int _boxIndex;
    private int _stage;

    public override async Task InitRequest()
    {
        _depo = _storage.GetDepo();        
        await CreateMessage(GetDepoCellListText(), OperationMarkup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessOperationSlide(slideId);
                    // 1, 3, 4, #
                }
                break;

            case 1:
                if (processing)
                {
                    await ProcessBoxStatsSlide(slideId, _currentBox);
                }
                else
                {
                    await ChangeMessage($"<b>СХРОН {_depo[_depoCellIndex].Number} {_currentDepoCellSide.ToString().ToUpper()}</b>\n" +
                    $"{_currentBox.StatsToString()}>Кол-во: ",
                    _digitKeyboardSlidePack[0].Markup());
                    _stage = 2;
                }
                break;

            case 2:
                if (processing)
                {
                    _currentBox.Count = await ProcessDigitKeyboardSlide(slideId, _currentBox.Count);
                }
                else
                {
                    await ProcessBoxAdding();
                    // 0
                }
                break;

            case 3:
                if (processing)
                {
                    _depoCellNumber = await ProcessDigitKeyboardSlide(slideId, _depoCellNumber);
                }
                else
                {
                    _depoCellIndex = GetDepoCellIndexByNumber(_depoCellNumber);
                    _currentDepoCellSide = DepoCellSide.Левая;
                    await ChangeMessage(GetDepoCellListText(), OperationMarkup());
                    _stage = 0;
                }
                break;

            case 4:
                if (processing)
                {
                    _boxIndex = await ProcessDigitKeyboardSlide(slideId, _boxIndex);
                }
                else
                {
                    _currentBox = GetBoxByIndex(_boxIndex);
                    await ChangeMessage($"{_currentBox.FullToString()}",
                        BoxOperationMarkup());
                    _stage = 5;
                }
                break;

            case 5:
                if (processing)
                {
                    await ProcessBoxOperationSlide(slideId);
                    //0, 6, 7, 8
                }
                break;

            case 6:
                if (processing)
                {
                    await ProcessBoxStatsSlide(slideId, _currentBox);
                }
                else
                {
                    await Report($"🔄{_currentBox.FullToString()}");
                    _depo = _storage.GetDepo();
                    await ChangeMessage(GetDepoCellListText(), OperationMarkup());
                    _stage = 0;
                }
                break;

            case 7:
                if (processing)
                {
                    _currentBox.Count = await ProcessDigitKeyboardSlide(slideId, _currentBox.Count);
                }
                else
                {
                    await Report($"🔄{_currentBox.FullToString()}");
                    _depo = _storage.GetDepo();
                    await ChangeMessage(GetDepoCellListText(), OperationMarkup());
                    _stage = 0;
                }
                break;

            case 8:
                if (processing)
                {
                    await ProcessAdresSlide(slideId, _newAdress);
                }
                else
                {                    
                    await Report($"{_currentBox.StatsToString()}<b>{_currentBox.Adress.ToString()}➡️{_newAdress.ToString()}</b>\n");
                    _currentBox.Adress = _newAdress;
                    _depo = _storage.GetDepo();

                    await ChangeMessage(GetDepoCellListText(), OperationMarkup());
                    _stage = 0;
                }
                break;
        }
    }
    
    private string GetDepoCellListText(bool isNumbered = false)
    {
        return _depo[_depoCellIndex].ToString(_currentDepoCellSide, isNumbered);
    }

    private async Task Report(string newReport)
    {
        _report.Append(newReport);

        if (NullScreenUnit)
            await CreateScreenUnit(Screen.Склад, _report.ToString());
        else
            await ChangeScreenUnit(_report.ToString());
    }

    private InlineKeyboardMarkup OperationMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("⬅️", $"{Id} {1}"),
                            new InlineKeyboardButton("➡️", $"{Id} {2}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("⏩🔢", $"{Id} {3}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("➕📦", $"{Id} {4}"),
                            new InlineKeyboardButton("🔁📦", $"{Id} {5}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("❌ Закрыть", $"{Id} {6}")
                        }
                });
    }

    private async Task ProcessOperationSlide(int slideId)
    {
        if (slideId == 6)
        {
            await CloseRequest();
            return;
        }

        switch (slideId)
        {
            case 1:
                await OnLeftArrow();
                break;

            case 2:
                await OnRightArrow();
                break;

            case 3:
                _depoCellNumber = 0;
                await ChangeMessage(">Перейти в схрон номер: ", _digitKeyboardSlidePack[0].Markup());
                _stage = 3;
                break;

            case 4:
                _currentBox = new Box();
                await ChangeMessage($"<b>СХРОН {_depo[_depoCellIndex].Number} {_currentDepoCellSide.ToString().ToUpper()}</b>\n>Добавить коробку\n", _boxStatsSlidePack[0].Markup());
                _stage = 1;
                break;

            case 5:
                _boxIndex = 0;
                await ChangeMessage(GetDepoCellListText(true) + "\n\n>Индекс коробки: ",
                    _digitKeyboardSlidePack[0].Markup());
                _stage = 4;
                break;
        }
    }

    private async Task OnLeftArrow()
    {
        if (_currentDepoCellSide == DepoCellSide.Левая)
        {
            _currentDepoCellSide = DepoCellSide.Правая;

            if (_depoCellIndex > 0)
                _depoCellIndex--;
            else
                _depoCellIndex = _depo.Count - 1;
        }
        else
            _currentDepoCellSide--;

            await ChangeMessage(GetDepoCellListText(), OperationMarkup());
    }

    private async Task OnRightArrow()
    {
        if (_currentDepoCellSide == DepoCellSide.Правая)
        {
            _currentDepoCellSide = DepoCellSide.Левая;

            if (_depoCellIndex < _depo.Count - 1)
                _depoCellIndex++;
            else
                _depoCellIndex = 0;
        }
        else
            _currentDepoCellSide++;

        await ChangeMessage(GetDepoCellListText(), OperationMarkup());
    }    

    private async Task ProcessBoxAdding()
    {
        _currentBox.Adress.DepoCellNumber = _depo[_depoCellIndex].Number;
        _currentBox.Adress.DepoCellSide = _currentDepoCellSide;
        _storage.PutBox(_currentBox);
        await Report($"➕{_currentBox.FullToString()}");
        _depo = _storage.GetDepo();

        await ChangeMessage(GetDepoCellListText(), OperationMarkup());
        _stage = 0;
    }

    private int GetDepoCellIndexByNumber(int depoCellNumber)
    {
        for (int i = 0; i < _depo.Count; i++)
        {
            if (_depo[i].Number == depoCellNumber)
                return i;                      
        }

        return 0;
    }

    private Box GetBoxByIndex(int index)
    {
        List<Box> boxesList = _depo[_depoCellIndex].GetBoxList(_currentDepoCellSide);
        index = index >= boxesList.Count ? boxesList.Count - 1 : index;

        return boxesList[index];
    }

    private InlineKeyboardMarkup BoxOperationMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("➖📦", $"{Id} {1}"),
                            new InlineKeyboardButton("🔄📦", $"{Id} {2}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("🔄Кол-во", $"{Id} {3}"),
                            new InlineKeyboardButton("🔄Адрес", $"{Id} {4}")
                        }
                });
    }

    public async Task ProcessBoxOperationSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                _storage.RemoveBox(_currentBox);
                await Report($"➖{_currentBox.FullToString()}");
                _depo = _storage.GetDepo();

                await ChangeMessage(GetDepoCellListText(), OperationMarkup());
                _stage = 0;
                break;

            case 2:
                await ChangeMessage($"{_currentBox.FullToString()}>Поменять характеристики:\n",
                    _boxStatsSlidePack[0].Markup());
                _currentBox.Reset();
                _stage = 6;
                break;

            case 3:
                await ChangeMessage($"{_currentBox.FullToString()}>Поменять кол-во: ",
                    _digitKeyboardSlidePack[0].Markup());
                _currentBox.Count = 0;
                _stage = 7;
                break;

            case 4:
                _adresSlidePack.Reset();
                await ChangeMessage($"{_currentBox.FullToString()}Поменять адрес: ",
                    _adresSlidePack[0].Markup());
                _newAdress = new Adres();
                _stage = 8;
                break;
        }
    }

    public override Task EnterText(string text)
    {
        throw new NotImplementedException();
    }
}