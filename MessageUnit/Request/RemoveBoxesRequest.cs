using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class RemoveBoxesRequest : Request
{
    private Box _boxSample = new Box();
    private int _boxesCount;
    private List<Box> _foundedBoxes = new List<Box>();
    private StringBuilder _report = new StringBuilder();

    private int _stage;     

    public override async Task InitRequest()
    {
        await CreateMessage(">Выбери текстиль:\n", _boxStatsSlidePack[0].Markup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessBoxStatsSlide(slideId, _boxSample);
                }
                else
                {
                    await ChangeMessage($"{_boxSample.StatsToString()}>Кол-во коробок: ",
                    _digitKeyboardSlidePack[0].Markup());
                    _stage = 1;
                }
                break;

            case 1:
                if (processing)
                {
                    _boxesCount = await ProcessDigitKeyboardSlide(slideId, _boxesCount);
                }
                else
                {
                    await ProcessFoundedBoxes();
                    // 2
                }
                break;

            case 2:
                if (processing)
                {
                    await ProcessOperationSlide(slideId);
                    // 0, 2
                }
                break;
        }
    }

    private string GetBoxListText()
    {
        var text = new StringBuilder();

        if(_foundedBoxes.Count == 0)        
            text.Append($"{_boxSample.StatsToString()}- нет -\n");        
        else        
            foreach ( var box in _foundedBoxes )            
                text.Append(box.FullToString());                  

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

    private async Task ProcessFoundedBoxes()
    {
        _foundedBoxes = new List<Box>();
        int currentBoxesCount = 0;

        foreach (var box in _storage.GetAllBoxes())
        {
            if (currentBoxesCount >= _boxesCount)
                break;

            if (box.Match(_boxSample))
            {
                _foundedBoxes.Add(box);
                currentBoxesCount++;
            }
        }

        await ChangeMessage(GetBoxListText(), OperationMarkup());
        _stage = 2;
    }

    private InlineKeyboardMarkup OperationMarkup()
    {
        var buttonsList = new List<List<InlineKeyboardButton>>();

        if(_foundedBoxes.Count > 0)
            buttonsList.Add(new() { new("➖📦", $"{Id} {1}") });

        buttonsList.Add(new() { new("Дальше ▶️", $"{Id} {2}") });
        buttonsList.Add(new() { new("❌ Закрыть", $"{Id} {3}") });

        return new InlineKeyboardMarkup(buttonsList);
    }

    private async Task ProcessOperationSlide(long slideId)
    {
        var text = new StringBuilder();

        switch (slideId)
        {
            case 1:
                foreach (var box in _foundedBoxes)
                {
                    _storage.RemoveBox(box);
                    text.Append($"➖{box.FullToString()}");
                }

                _foundedBoxes = new List<Box>();
                await Report(text.ToString());
                await ChangeMessage(text.ToString(), OperationMarkup());
                _stage = 2;
                break;

            case 2:
                _boxSample = new Box();
                _boxesCount = 0;
                await ChangeMessage(">Выбери текстиль:\n", _boxStatsSlidePack[0].Markup());
                _stage = 0;
                break;

            case 3:
                await CloseRequest();
                break;
        }
    }

    public override Task EnterText(string text)
    {
        throw new NotImplementedException();
    }
}