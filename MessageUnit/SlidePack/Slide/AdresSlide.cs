using Telegram.Bot.Types.ReplyMarkups;

class AdresSlide : Slide
{
    public Action<Adres> ProcessAdress;

    public AdresSlide(Func<string, string> processText, Action< Adres> processAdress, Func<InlineKeyboardMarkup> markup) : base(processText, markup)
    {
        ProcessAdress = processAdress;
    }
}
