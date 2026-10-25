using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

class Terminal
{
    private ITelegramBotClient _recipient;    

    public void Set(ITelegramBotClient recipient)
    {
        _recipient = recipient;
    }

    public async Task SendMessage(long senderId, string text, ReplyKeyboardMarkup replyMarkup = null)
    {
        await _recipient.SendMessage(senderId, text, ParseMode.Html, replyMarkup: replyMarkup);
    }

    public async Task SendForwardedMessage(long senderId, long fromSenderId, int messageId)
    {
        await _recipient.ForwardMessage(senderId, fromSenderId, messageId);
    }

    public async Task<Message> CreateMessage(long senderId, string text, InlineKeyboardMarkup markup)
    {
        return await _recipient.SendMessage(senderId, text, ParseMode.Html, replyMarkup: markup);              
    }

    public async Task ChangeMessage(long senderId, int messageId, string text, InlineKeyboardMarkup markup = null)
    {
        await _recipient.EditMessageText(senderId, messageId, text, ParseMode.Html, replyMarkup: markup);        
    }

    public async Task DeleteMessage(long senderId, int messageId)
    {
        try
        {
            await _recipient.DeleteMessage(senderId, messageId);
        }
        catch
        {
            Console.WriteLine("Удаление сообщения неуспешно");
        }
    }
}