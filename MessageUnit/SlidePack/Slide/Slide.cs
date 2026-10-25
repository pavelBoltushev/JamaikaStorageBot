using Telegram.Bot.Types.ReplyMarkups;

class Slide
{
    public Func<string, string> ProcessText;
    public Func<InlineKeyboardMarkup> Markup;

    public Slide(Func<string, string> processText, Func<InlineKeyboardMarkup> markup)
    {
        ProcessText = processText;
        Markup = markup;
    }
}