using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class ReceptionAdmissionRequest : Request
{
    private List<Supply> _suppliesList = new List<Supply>();
    private Supply _newSupply = new Supply();
    private Box _currentBox = new Box();
    private StringBuilder _report = new StringBuilder();

    private int _supplyIndex;
    private int _boxIndex;
    private int _stage;

    public override async Task InitRequest()
    {
        _suppliesList = _storage.GetReception();
        await CreateMessage(GetSupplyListText(), OperationMarkup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessOperationSlide(slideId);
                    // 10, 3, 6, 8
                }
                break;

            case 10:
                if (processing)
                {
                    _newSupply.Number = await ProcessDigitKeyboardSlide(slideId, _newSupply.Number);
                }
                else
                {
                    await AddLogToMessage("\n>Характеристика:\n", _supplyStatsSlidePack[0].Markup());
                    _stage = 1;
                }
                break;

            case 1:
                if (processing)
                {
                    await ProcessSupplyStatsSlide(slideId, _newSupply.Stats);
                }
                else
                {
                    await AddLogToMessage(">Кол-во: ", _digitKeyboardSlidePack[0].Markup());
                    _stage = 2;
                }
                break;

            case 2:
                if (processing)
                {
                    _newSupply.Stats.Count = await ProcessDigitKeyboardSlide(slideId, _newSupply.Stats.Count);
                }
                else
                {
                    _suppliesList.Add(_newSupply);
                    await CreateScreenUnit(Screen.Склад, $"➕{_newSupply.ToString()}");
                    SaveLoader.Save(_suppliesList);
                    _supplyIndex = _suppliesList.Count - 1;
                    await ChangeMessage(GetSupplyListText(), OperationMarkup());
                    _stage = 0;
                }
                break;

            case 3:
                if (processing)
                {
                    await ProcessSizeSlide(slideId, _currentBox);
                }
                else
                {
                    await AddLogToMessage(">Кол-во: ", _digitKeyboardSlidePack[0].Markup());
                    _stage = 4;
                }
                break;

            case 4:
                if (processing)
                {
                    _currentBox.Count = await ProcessDigitKeyboardSlide(slideId, _currentBox.Count);
                }
                else
                {
                    _suppliesList[_supplyIndex].Boxes.Add(_currentBox);
                    SaveLoader.Save(_suppliesList);
                    await ChangeMessage(_suppliesList[_supplyIndex].ToString(), ConfirmationMarkup());
                    _stage = 5;
                }
                break;

            case 5:
                if (processing)
                {
                    await ProcessConfirmationSlide(slideId);
                    // 0, 3
                }
                break;

            case 11:
                if (processing)
                {
                    await ProcessUnpackingFinishSlide(slideId);                    
                }
                break;

            case 6:
                if (processing)
                {
                    _boxIndex = await ProcessDigitKeyboardSlide(slideId, _boxIndex);
                }
                else
                {
                    _currentBox = GetBoxByIndex(_boxIndex);
                    await ChangeMessage(_suppliesList[_supplyIndex].RemoveMarkedToString(_currentBox), ExecutionMarkup());
                    _stage = 7;
                }
                break;

            case 7:
                if (processing)
                {
                    await ProcessExecutionSlide(slideId);
                    // 0, 9
                }
                break;

            case 8:
                if (processing)
                {
                    await ProcessAdresSlide(slideId, _currentBox.Adress);
                }
                else
                {
                    await ProcessBoxAdding();
                    // 8, 9
                }
                break;

            case 9:
                if (processing)
                {
                    await ProcessSpreadFinishSlide(slideId);
                    // 0
                }
                break;
        }
    }

    private string GetSupplyListText()
    {
        if (_suppliesList.Count == 0)
            return "- приёмка пуста -";
        else
        {
            string supplyText = _suppliesList[_supplyIndex].ToString();
            int count = _suppliesList.Count;
            int currNum = _supplyIndex + 1;
            return $"{supplyText}({currNum}/{count})";
        }
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
        var buttonsList = new List<List<InlineKeyboardButton>>();

        if(_suppliesList.Count != 0)
        {
            if (_suppliesList.Count > 1)
                buttonsList.Add(new () { new ("⬅️", $"{Id} {1}"), new ("➡️", $"{Id} {2}") });

            if (!_suppliesList[_supplyIndex].IsUnpacked)
                buttonsList.Add(new() { new("▶️ Разобрать", $"{Id} {3}") });
            else
            {
                buttonsList.Add(new() { new("➖📦", $"{Id} {4}") });
                buttonsList.Add(new() { new("▶️ Разложить", $"{Id} {5}") });
            }
        }

        buttonsList.Add(new() { new("➕ ПОСТАВКА", $"{Id} {6}")});
        buttonsList.Add(new() { new("❌ Закрыть", $"{Id} {7}") });
        return new InlineKeyboardMarkup(buttonsList);
    }

    public async Task ProcessOperationSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                if (_supplyIndex == 0)
                    _supplyIndex = _suppliesList.Count - 1;
                else
                    _supplyIndex--;

                await ChangeMessage(GetSupplyListText(), OperationMarkup());
                    break;

            case 2:
                if (_supplyIndex == _suppliesList.Count - 1)
                    _supplyIndex = 0;
                else
                    _supplyIndex++;

                await ChangeMessage(GetSupplyListText(), OperationMarkup());
                break;

            case 3:
                _suppliesList[_supplyIndex].IsUnpacked = true;
                SaveLoader.Save(_suppliesList);
                _currentBox = new Box();
                _currentBox.SetSupplyStats(_suppliesList[_supplyIndex].Stats);
                _currentBox.SupplyNumber = _suppliesList[_supplyIndex].Number;
                _currentBox.Adress.isReception = true;
                await ChangeMessage($"{_suppliesList[_supplyIndex].ToString()}>Размер:\n", _sizeSlidePack[0].Markup());
                _stage = 3;
                break;

            case 4:
                _boxIndex = 0;
                await ChangeMessage($"{_suppliesList[_supplyIndex].NumberedToString()}\nСнять коробку\n>Индекс: ",
                    _digitKeyboardSlidePack[0].Markup());
                _stage = 6;
                break;

            case 5:
                _currentBox = _suppliesList[_supplyIndex].Boxes[0];
                _currentBox.Adress.isReception = false;
                await ChangeMessage($"{_currentBox.StatsToString()}>Адрес: ", _adresSlidePack[0].Markup());
                _stage = 8;
                break;

            case 6:
                _newSupply = new Supply();
                await ChangeMessage("Новая поставка\n>Номер поставки: ", _digitKeyboardSlidePack[0].Markup());
                _stage = 10;
                break;

            case 7:
                await CloseRequest();
                break;
        }
    }

    private InlineKeyboardMarkup ConfirmationMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton($"🫴📦", $"{Id} {1}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("⛔️ Достаточно", $"{Id} {0}")
                        }
                });
    }

    private async Task ProcessConfirmationSlide(long slideId)
    {
        if (slideId > 0)
        {
            _currentBox = new Box();
            _currentBox.SetSupplyStats(_suppliesList[_supplyIndex].Stats);
            _currentBox.SupplyNumber = _suppliesList[_supplyIndex].Number;
            _currentBox.Adress.isReception = true;
            await ChangeMessage($"{_suppliesList[_supplyIndex].ToString()}>Размер:\n", _sizeSlidePack[0].Markup());
            _stage = 3;
        }
        else
        {
            int balance = _suppliesList[_supplyIndex].Stats.Count - _suppliesList[_supplyIndex].Boxes.Sum(box => box.Count);
            await ChangeMessage($"Остаток: {balance}", UnpackingFinishMarkup());
            _stage = 11;
        }
    }

    private InlineKeyboardMarkup UnpackingFinishMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton($"✅ Ок", $"{Id} {1}")
                        }
                });
    }

    private async Task ProcessUnpackingFinishSlide(long slideId)
    {
        await ChangeMessage(GetSupplyListText(), OperationMarkup());
        _stage = 0;
    }

    private Box GetBoxByIndex(int index)
    {
        List<Box> boxesList = _suppliesList[_supplyIndex].Boxes;
        index = index >= boxesList.Count ? boxesList.Count - 1 : index;

        return boxesList[index];
    }

    private InlineKeyboardMarkup ExecutionMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton($"✅ Подтвердить", $"{Id} {1}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("◀️ Назад", $"{Id} {0}")
                        }
                });
    }

    private async Task ProcessExecutionSlide(long slideId)
    {
        if (slideId > 0)
        {
            _suppliesList[_supplyIndex].Boxes.Remove(_currentBox);
            SaveLoader.Save(_suppliesList);
        }

        if (_suppliesList[_supplyIndex].Boxes.Count == 0)
        {
            await ChangeMessage("- поставка пуста -", SpreadFinishMarkup());
            _stage = 9;
        }
        else
        {
            await ChangeMessage(GetSupplyListText(), OperationMarkup());
            _stage = 0;
        }
    }

    private async Task ProcessBoxAdding()
    {
        _suppliesList[_supplyIndex].Boxes.Remove(_currentBox);
        SaveLoader.Save(_suppliesList);
        _storage.PutBox(_currentBox);
        await Report($"➕{_currentBox.FullToString()}");

        if (_suppliesList[_supplyIndex].Boxes.Count == 0)
        {
            await ChangeMessage("- поставка пуста -", SpreadFinishMarkup());
            _stage = 9;
        }
        else
        {
            _currentBox = _suppliesList[_supplyIndex].Boxes[0];
            _currentBox.Adress.isReception = false;
            _adresSlidePack.Reset();
            await ChangeMessage($"{_currentBox.StatsToString()}>Адрес: ", _adresSlidePack[0].Markup());
            _stage = 8;
        }
    }

    private InlineKeyboardMarkup SpreadFinishMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton($"✅ Ок", $"{Id} {1}")
                        }
                });
    }

    private async Task ProcessSpreadFinishSlide(long slideId)
    {
        _suppliesList.RemoveAt(_supplyIndex);
        SaveLoader.Save(_suppliesList);
        _supplyIndex = 0;
        await ChangeMessage(GetSupplyListText(), OperationMarkup());
        _stage = 0;
    }

    public override Task EnterText(string text)
    {
        throw new NotImplementedException();
    }
}