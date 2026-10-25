using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class StudioShipmentRequest : Request
{
    private StudioShipment _studioShipment = new StudioShipment();
    private Studio _newStudio = new Studio();

    private int _studioIndex;
    private int _stage;

    private Studio CurrentStudio => _studioShipment.StudiosList[_studioIndex];

    public override async Task InitRequest()
    {
        _studioShipment = _storage.GetStudioShipment();
        SetScreenUnit(_studioShipment.ScreenUnit);
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
                    await ProcessStudioAdresSlide(slideId);                    
                }
                break;

            case 2:
                if (processing)
                {
                    _newStudio.Count = await ProcessDigitKeyboardSlide(slideId, _newStudio.Count);                    
                }
                else
                {
                    await AddNewStudio();
                }
                break;
        }
    }

    private string GetShipmentText(bool admissionText = false)
    {
        var text = new StringBuilder();
        text.Append($"📺 Студии  {GeneralCountToString()}\n" +
            $" - - - - - - - - - - - - - - - - - - - - - \n");

        if (_studioShipment.StudiosList.Count == 0)
            text.Append("- отгрузка пуста -");
        else
        {
            for (int i = 0; i < _studioShipment.StudiosList.Count; i++)
            {
                if(admissionText)
                    text.Append((i == _studioIndex ? "♦️" : ""));

                text.Append($"{_studioShipment.StudiosList[i].ToString()}\n");
            }
        }

        text.Append($"\nОбновление: {DateTime.Now.ToString("HH:mm")}");

        return text.ToString();
    }

    private string GeneralCountToString()
    {
        int boxesCount = 0;
        int packsCount = 0;

        foreach (var studio in _studioShipment.StudiosList)
        {
            if (studio.Count == 0)
                packsCount++;
            else
                boxesCount += studio.Count;
        }

        return $"{(boxesCount != 0 ? $"📦 - {boxesCount}  " : "")}" +
            $"{(packsCount != 0 ? $"🏷 - {packsCount}" : "")}";
    }

    private InlineKeyboardMarkup OperationMarkup()
    {        
        var buttonsList = new List<List<InlineKeyboardButton>>();

        if(_studioShipment.StudiosList.Count > 1)
            buttonsList.Add(new() { new("⬆️", $"{Id} {1}"), new("⬇️", $"{Id} {2}") });

        if(_studioShipment.StudiosList.Count != 0)
        {
            if (CurrentStudio.Count != 0)
                buttonsList.Add(new() { new("-📦", $"{Id} {3}"), new("+📦", $"{Id} {4}") });
            else
                buttonsList.Add(new() { new("+📦", $"{Id} {4}") });

            switch (CurrentStudio.Status)
            {
                case Status.Сборка:
                    buttonsList.Add(new() { new("🔡 >>", $"{Id} {6}") });
                    break;

                case Status.Скан:
                    buttonsList.Add(new() { new("<< 🫳", $"{Id} {5}"), new("🚚 >>", $"{Id} {7}") });
                    break;

                case Status.Отгрузка:
                    buttonsList.Add(new() { new("<< 🔡", $"{Id} {6}"), new("✅ >>", $"{Id} {8}") });
                    break;

                case Status.Выполнен:
                    buttonsList.Add(new() { new("<< 🚚", $"{Id} {7}") });
                    break;
            }

            buttonsList.Add(new() { new("🔈 Оповестить", $"{Id} {9}") });

            if(_studioShipment.StudiosList.Any(studio => studio.Status == Status.Выполнен))
                buttonsList.Add(new() { new("✅ Отгрузить", $"{Id} {10}") });
        }        
        
        buttonsList.Add(new() { new("➕ СТУДИЯ", $"{Id} {11}") });
        buttonsList.Add(new() { new("❌ Закрыть", $"{Id} {12}") });        

        return new InlineKeyboardMarkup(buttonsList);
    }

    private async Task ProcessOperationSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                _studioIndex = _studioIndex == 0 ? _studioShipment.StudiosList.Count - 1 : --_studioIndex;
                break;

            case 2:
                _studioIndex = _studioIndex == _studioShipment.StudiosList.Count - 1 ? 0 : ++_studioIndex;
                break;

            case 3:
                CurrentStudio.Count--;
                break;

            case 4:
                CurrentStudio.Count++;
                break;

            case 5:
                CurrentStudio.Status = Status.Сборка;
                break;

            case 6:
                CurrentStudio.Status = Status.Скан;
                break;

            case 7:
                CurrentStudio.Status = Status.Отгрузка;
                break;

            case 8:
                CurrentStudio.Status = Status.Выполнен;
                break;

            case 9:
                await ReCreateScreenUnit(GetShipmentText());
                await AddLogToMessage("\n🔈", OperationMarkup());
                return;

            case 10:
                await Ship();
                return;

            case 11:
                _newStudio = new Studio();
                await ChangeMessage("НОВАЯ СТУДИЯ\n>Адрес:\n", StudioAdresMarkup());
                _stage = 1;
                return;

            case 12:
                await CloseRequest();
                return;
        }

        SaveLoader.Save(_studioShipment);
        await ChangeScreenUnit(GetShipmentText());
        await ChangeMessage(GetShipmentText(true), OperationMarkup());
    }

    private async Task Ship()
    {
        for (int i = 0; i < _studioShipment.StudiosList.Count; i++)
        {
            var currentStudio = _studioShipment.StudiosList[i];

            if (currentStudio.Status == Status.Выполнен)
            {                
                await SendReport(Report.Студии, currentStudio.ToString());
                _studioShipment.StudiosList.Remove(currentStudio);
                i--;
            }
        }

        if (_studioShipment.StudiosList.Count == 0)
        {
            await DeleteScreenUnit();
            _studioShipment.ScreenUnit = null;
        }
        else
            await ChangeScreenUnit(GetShipmentText());

        SaveLoader.Save(_studioShipment);
        await ChangeMessage(GetShipmentText(true), OperationMarkup());
    }

    private InlineKeyboardMarkup StudioAdresMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Марата", $"{Id} {1}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Коломяжский", $"{Id} {2}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Сено", $"{Id} {6}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Новоспасский", $"{Id} {3}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Весковский", $"{Id} {4}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Талалихина", $"{Id} {5}")
                        },
                });
    }

    private async Task ProcessStudioAdresSlide(int slideId)
    {
        string log = "";

        switch (slideId)
        {
            case 1:
                _newStudio.Adress = StudioAdress.Марата;
                log += "Марата\n";
                break;

            case 2:
                _newStudio.Adress = StudioAdress.Коломяжский;
                log += "Коломяжский\n";
                break;

            case 3:
                _newStudio.Adress = StudioAdress.Новоспасский;
                log += "Новоспасский\n";
                break;

            case 4:
                _newStudio.Adress = StudioAdress.Весковский;
                log += "Весковский\n";
                break;

            case 5:
                _newStudio.Adress = StudioAdress.Талалихина;
                log += "Талалихина\n";
                break;

            case 6:
                _newStudio.Adress = StudioAdress.Сено;
                log += "Сено\n";
                break;
        }

        log += ">Кол-во мест: \n";

        await AddLogToMessage(log, _digitKeyboardSlidePack[0].Markup());
        _stage = 2;
    }

    private async Task AddNewStudio()
    {
        _newStudio.Status = Status.Сборка;
        _studioShipment.StudiosList.Add(_newStudio);
        _studioIndex = _studioShipment.StudiosList.Count - 1;

        if (NullScreenUnit)
            _studioShipment.ScreenUnit = await CreateScreenUnit(Screen.Студии, GetShipmentText());
        else
            await ChangeScreenUnit(GetShipmentText());

        await ChangeMessage(GetShipmentText(true), OperationMarkup());
        SaveLoader.Save(_studioShipment);
        _stage = 0;
    }

    public override Task EnterText(string text)
    {
        throw new NotImplementedException();
    }
}