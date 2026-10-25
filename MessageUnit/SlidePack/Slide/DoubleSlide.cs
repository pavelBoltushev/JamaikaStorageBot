using Telegram.Bot.Types.ReplyMarkups;

class DoubleSlide : Slide
{
    public Func<double, double> ProcessDouble;

    public DoubleSlide(Func<string, string> processText, Func<double, double> processDouble, Func<InlineKeyboardMarkup> markup) : base(processText, markup)
    {
        ProcessDouble = processDouble;
    }
}