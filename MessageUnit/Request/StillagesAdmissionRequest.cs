using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class StillagesAdmissionRequest : Request
{       
    private List<List<List<Box>>> _storageList = new List<List<List<Box>>>();
    private Box _currentBox = new Box();
    private Adres _newAdress = new Adres();
    private StringBuilder _report = new StringBuilder();

    private int _rowIndex;
    private int _stillageIndex;
    private int _boxIndex;
    private int _stage;

    public override async Task InitRequest()
    {              
        _storageList = _storage.GetStorageList();
        await CreateMessage(GetStillageListText(), OperationMarkup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessOperationSlide(slideId);
                    // 1, 4, #
                }
                break;

            case 1:
                if (processing)
                {
                    await ProcessBoxStatsSlide(slideId, _currentBox);
                }
                else
                {
                    await ChangeMessage($"<b>{_rowIndex + 1}.{_stillageIndex + 1}</b>\n{_currentBox.StatsToString()}>Кол-во: ",
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
                    await ChangeMessage($"<b>{_rowIndex + 1}.{_stillageIndex + 1}</b>\n{_currentBox.StatsToString()}>Место:\n",
                    Letter6Markup());
                    _stage = 3;
                }
                break;

            case 3:
                if (processing)
                {
                    await ProcessLetter6Slide(slideId);
                    // 0
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
                    await ChangeMessage(_currentBox.FullToString(),
                        BoxOperationMarkup());
                    _stage = 5;
                }
                break;

            case 5:
                if (processing)
                {
                    await ProcessBoxOperationSlide(slideId);
                    // 0, 6, 7, 8
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
                    _storageList = _storage.GetStorageList();
                    await ChangeMessage(GetStillageListText(), OperationMarkup());
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
                    _storageList = _storage.GetStorageList();
                    await ChangeMessage(GetStillageListText(), OperationMarkup());
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
                    _storageList = _storage.GetStorageList();

                    await ChangeMessage(GetStillageListText(), OperationMarkup());
                    _stage = 0;
                }
                break;
        }
    }

    private string GetStillageListText(bool numbered = false)
    {
        int rowNumber = _rowIndex + 1;
        int stillageNumber = _stillageIndex + 1;
        var text = new StringBuilder();
        var stillageList = _storageList[_rowIndex][_stillageIndex];
        text.Append($"<b>{rowNumber}.{stillageNumber}  📦 {stillageList.Count}</b>\n");

        if (stillageList.Count == 0)
        {
            text.Append("- пусто -");
            return text.ToString();
        }

        for (int i = 0; i < stillageList.Count; i++)
        {
            if (numbered)
                text.Append($"[{i}]");

            text.Append($"<b>{stillageList[i].Adress.Placement}</b>{stillageList[i].StatsToString()}");
        }

        text.Append($"({rowNumber}.{stillageNumber})");
        return text.ToString();
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
                            new InlineKeyboardButton("⏪", $"{Id} {3}"),
                            new InlineKeyboardButton("⏩", $"{Id} {4}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("➕📦", $"{Id} {5}"),
                            new InlineKeyboardButton("🔁📦", $"{Id} {6}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("❌ Закрыть", $"{Id} {7}")
                        },
                });
    }

    public async Task ProcessOperationSlide(int slideId)
    {
        if(slideId == 7)
        {
            await CloseRequest();
            return;
        }

        switch (slideId)
        {
            case 1:
                if (_stillageIndex > 0)
                    _stillageIndex--;
                else
                    _stillageIndex = _storageList[_rowIndex].Count - 1;

                break;

            case 2:
                if (_stillageIndex < _storageList[_rowIndex].Count - 1)
                    _stillageIndex++;
                else
                    _stillageIndex = 0;

                break;

            case 3:
                if (_rowIndex > 0)
                    _rowIndex--;
                else
                    _rowIndex = _storageList.Count - 1;

                _stillageIndex = 0;
                break;

            case 4:
                if (_rowIndex < _storageList.Count - 1)
                    _rowIndex++;
                else
                    _rowIndex = 0;

                _stillageIndex = 0;
                break;

            case 5:
                _currentBox = new Box();
                await ChangeMessage($"<b>{_rowIndex + 1}.{_stillageIndex + 1}</b>\n>Добавить коробку\n", _boxStatsSlidePack[0].Markup());
                _stage = 1;
                return;

            case 6:
                _boxIndex = 0;
                await ChangeMessage(GetStillageListText(true) + "\n>Индекс коробки: ",
                    _digitKeyboardSlidePack[0].Markup());
                _stage = 4;
                return;
        }

        await ChangeMessage(GetStillageListText(), OperationMarkup());
    }

    private InlineKeyboardMarkup Letter6Markup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Е", $"{Id} {1}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Д", $"{Id} {2}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Г", $"{Id} {3}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("В", $"{Id} {4}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Б", $"{Id} {5}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("А", $"{Id} {6}")
                        }
                });
    }

    public async Task ProcessLetter6Slide(int slideId)
    {
        string placement = "";

        switch (slideId)
        {
            case 1:
                placement = "Е";
                break;

            case 2:
                placement = "Д";
                break;

            case 3:
                placement = "Г";
                break;

            case 4:
                placement = "В";
                break;

            case 5:
                placement = "Б";
                break;

            case 6:
                placement = "А";
                break;
        }

        var adress = new Adres();
        adress.Row = _rowIndex + 1;
        adress.Stillage = _stillageIndex + 1;
        adress.Placement = placement;
        _currentBox.Adress = adress;
        _storage.PutBox(_currentBox);
        await Report($"➕{_currentBox.FullToString()}");
        _storageList = _storage.GetStorageList();

        await ChangeMessage(GetStillageListText(), OperationMarkup());
        _stage = 0;
    }

    private Box GetBoxByIndex(int index)
    {
        List<Box> boxesList = _storageList[_rowIndex][_stillageIndex];
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
                _storageList = _storage.GetStorageList();

                await ChangeMessage(GetStillageListText(), OperationMarkup());
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
                await ChangeMessage($"{_currentBox.FullToString()}>Поменять адрес: ",
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