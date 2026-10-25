using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class GetStatisticsRequest : Request
{
    private string _statistics = string.Empty;
    private Box _boxSample = new Box();
    private bool _published;

    private int _stage;

    public override async Task InitRequest()
    {
        await CreateMessage(">Выбери позицию:\n", _coreStatsSlidePack[0].Markup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessCoreStatsSlide(slideId, _boxSample);
                }
                else
                {
                    _statistics = GetStatistics(
                        $"<b>{_boxSample.StatisticsTitleToString()}</b>",
                        _boxSample.Type,
                        _boxSample.Model,
                        _boxSample.Textile);

                    await ChangeMessage(_statistics, ResultMarkup());
                    _stage = 1;
                }
                break;

            case 1:
                if (processing)
                {
                    await ProcessResultSlide(slideId);
                }
                break;
        }
    }

    private InlineKeyboardMarkup ResultMarkup()
    {
        var buttonsList = new List<List<InlineKeyboardButton>>();

        buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("Дальше ▶️", $"{Id} {1}") });

        if (!_published)
            buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("📺 Остатки", $"{Id} {2}") });

        buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("❌ Закрыть", $"{Id} {3}") });

        return new InlineKeyboardMarkup(buttonsList);
    }

    public async Task ProcessResultSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                _boxSample = new Box();
                _published = false;
                await ChangeMessage(">Выбери позицию:\n", _coreStatsSlidePack[0].Markup());
                _stage = 0;
                break;

            case 2:
                await CreateScreenUnit(Screen.Остатки, _statistics);
                _published = true;
                await AddLogToMessage("\n➡️📺 Остатки", ResultMarkup());
                break;

            case 3:
                await CloseRequest();
                break;
        }        
    }

    private string GetStatistics(string title, Type type, Model model, Textile textile)
    {
        var statistics = new StringBuilder();
        statistics.Append(title);

        for (var i = 0; i < Enum.GetValues<Color>().Length; i++)
        {
            for (var j = 0; j < Enum.GetValues<Size>().Length; j++)
            {
                int generalCount = 0;

                foreach (var box in _storage.ExtractBy(type, model, textile))
                {
                    if (box.Color == (Color)i && box.Size == (Size)j)
                    {
                        generalCount += box.Count;
                    }
                }

                if (generalCount != 0)
                    statistics.Append($"{((Color)i).ToString().ToLower()} {Box.SizeToString((Size)j)} - {generalCount}\n");
            }
        }

        return statistics.ToString();
    }

    public override Task EnterText(string text)
    {
        throw new NotImplementedException();
    }
}