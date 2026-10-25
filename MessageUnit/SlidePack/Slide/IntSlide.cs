using Telegram.Bot.Types.ReplyMarkups;

class IntSlide : Slide
{
    public Func<int, int> ProcessInt;

    public IntSlide(Func<string, string> processText, Func<int, int> processInt, Func<InlineKeyboardMarkup> markup) : base(processText, markup)
    {
        ProcessInt = processInt;
    }
}