using System.Text;
using Telegram.Bot.Types.ReplyMarkups;

class ReviewBoxesRequest : Request
{
    private List<Box> _reviewedBoxes = new List<Box>();    
    private Box _boxSample = new Box();
    private bool _positionReviewed;

    private int _pageIndex;
    private int _pagesCount;
    private int _stage;

    public override async Task InitRequest()
    {
        PutToReview(_storage.GetAllBoxes().ToList());
        await CreateMessage(GetCurrentPage(), ReviewMarkup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessReviewSlide(slideId);
                }
                break;

            case 1:
                if (processing)
                {
                    await ProcessCoreStatsSlide(slideId, _boxSample);
                }
                else
                {
                    PutToReview(_storage.ExtractBy(_boxSample.Type, _boxSample.Model, _boxSample.Textile).ToList());
                    await ChangeMessage(GetCurrentPage(), ReviewMarkup());
                    _stage = 0;
                }
                break;
        }
    }

    private void PutToReview(List<Box> boxes)
    {
        _reviewedBoxes = boxes;
        _reviewedBoxes.Sort(new StatsComparer());
        _pagesCount = _reviewedBoxes.Count % 10 == 0 ? _reviewedBoxes.Count / 10 : (_reviewedBoxes.Count / 10) + 1;
        _pageIndex = 0;
    }

    private string GetCurrentPage()
    {
        var text = new StringBuilder();

        if (_positionReviewed)
            text.Append($"<b>{_boxSample.StatisticsTitleToString()}</b>");
        else
            text.Append("<b>ВСЕ КОРОБКИ\n</b>");

        if (_reviewedBoxes.Count == 0)
        {
            text.Append("- нет -");
            return text.ToString();
        }

        text.Append($"📦 - {_reviewedBoxes.Count}  ({_pageIndex + 1}/{_pagesCount})\n\n");

        int currentIndex = _pageIndex * 10;

        for (int i = 0; i < 10; i++)
        {
            text.Append(_reviewedBoxes[currentIndex].FullToString());
            currentIndex++;

            if (currentIndex == _reviewedBoxes.Count)
                break;
        }
        
        return text.ToString();
    }

    private InlineKeyboardMarkup ReviewMarkup()
    {
        var buttonsList = new List<List<InlineKeyboardButton>>();

        buttonsList.Add(new List<InlineKeyboardButton>{ new InlineKeyboardButton("⬅️", $"{Id} {1}"),
                                                        new InlineKeyboardButton("➡️", $"{Id} {2}")});

        if (_positionReviewed)
            buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("Обзор ВСЕХ", $"{Id} {3}") });
        
        buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("Обзор по позиции", $"{Id} {4}") });
        buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("💾 В файл", $"{Id} {5}") });
        buttonsList.Add(new List<InlineKeyboardButton> { new InlineKeyboardButton("❌ Закрыть", $"{Id} {6}") });

        return new InlineKeyboardMarkup(buttonsList);
    }

    private async Task ProcessReviewSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                if(_pageIndex > 0)
                    _pageIndex--;
                else
                    _pageIndex = _pagesCount - 1;

                await ChangeMessage(GetCurrentPage(), ReviewMarkup());
                    break;

            case 2:
                if (_pageIndex < _pagesCount - 1)
                    _pageIndex++;
                else
                    _pageIndex = 0;

                await ChangeMessage(GetCurrentPage(), ReviewMarkup());
                break;

            case 3:
                _positionReviewed = false;
                PutToReview(_storage.GetAllBoxes().ToList());
                await ChangeMessage(GetCurrentPage(), ReviewMarkup());
                break;

            case 4:
                _positionReviewed = true;
                _boxSample = new Box();
                await ChangeMessage(">Выбери позицию:\n", _coreStatsSlidePack[0].Markup());
                _stage = 1;
                break;

            case 5:
                await ProcessFileSaving();
                break;

            case 6:
                await CloseRequest();
                break;
        }
    }

    private async Task ProcessFileSaving()
    {
        var text = new StringBuilder();
        string filename = string.Empty;
        int counter = 0;

        if (_positionReviewed)                    
            filename = $"{_boxSample.Type.ToString()}{_boxSample.Model.ToString()}{_boxSample.Textile.ToString()}";        
        else                   
            filename = "ВсеКоробки";

        foreach (var box in _reviewedBoxes)
        {
            text.Append(box.ReviewToSting());
            counter++;

            if(counter == 7)
            {
                text.AppendLine();
                counter = 0;
            }
        }
        
        File.WriteAllText($"data/review/{filename}.txt", text.ToString());
        await AddLogToMessage($"\n💾 {filename}", ReviewMarkup());
    }

    public override Task EnterText(string text)
    {
        throw new NotImplementedException();
    }
}