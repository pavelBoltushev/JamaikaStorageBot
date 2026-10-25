using Telegram.Bot.Types.ReplyMarkups;

class BoxStatsSlide : Slide
{
    public Action<Box> ProcessBox;

    public BoxStatsSlide(Func<string, string> processText, Action<Box> processBox, Func<InlineKeyboardMarkup> markup) : base(processText, markup)
    {
        ProcessBox = processBox;
    }
}