using Telegram.Bot.Types.ReplyMarkups;

class DigitKeyboardSlidePack
{
    private int _requestId;
    private Dictionary<int, IntSlide> _slides;

    public DigitKeyboardSlidePack(int requestId)
    {
        _requestId = requestId;
        _slides = FillSlides();
    }

    public IntSlide this[int index]
    {
        get => _slides[index];
    }

    private Dictionary<int, IntSlide> FillSlides()
    {
        return new ()
        {
            [0] = new(
                text => text,
                num => num,
                () => DigitMarkup()),

            [1] = new(
                text => text += "1",
                num => Plus(num, 1),
                () => DigitMarkup()),

            [2] = new(
                text => text += "2",
                num => Plus(num, 2),
                () => DigitMarkup()),

            [3] = new(
                text => text += "3",
                num => Plus(num, 3),
                () => DigitMarkup()),

            [4] = new(
                text => text += "4",
                num => Plus(num, 4),
                () => DigitMarkup()),

            [5] = new(
                text => text += "5",
                num => Plus(num, 5),
                () => DigitMarkup()),

            [6] = new(
                text => text += "6",
                num => Plus(num, 6),
                () => DigitMarkup()),

            [7] = new(
                text => text += "7",
                num => Plus(num, 7),
                () => DigitMarkup()),

            [8] = new(
                text => text += "8",
                num => Plus(num, 8),
                () => DigitMarkup()),

            [9] = new(
                text => text += "9",
                num => Plus(num, 9),
                () => DigitMarkup()),

            [10] = new(
                text => text += "0",
                num => Plus(num, 0),
                () => DigitMarkup()),

            [11] = new(
                text => DeleteLastDigit(text),
                num => Minus(num),
                () => DigitMarkup())
        };
    }

    private InlineKeyboardMarkup DigitMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("1", $"{_requestId} {1}"),
                            new InlineKeyboardButton("2", $"{_requestId} {2}"),
                            new InlineKeyboardButton("3", $"{_requestId} {3}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("4", $"{_requestId} {4}"),
                            new InlineKeyboardButton("5", $"{_requestId} {5}"),
                            new InlineKeyboardButton("6", $"{_requestId} {6}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("7", $"{_requestId} {7}"),
                            new InlineKeyboardButton("8", $"{_requestId} {8}"),
                            new InlineKeyboardButton("9", $"{_requestId} {9}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("◀️", $"{_requestId} {11}"),
                            new InlineKeyboardButton("0", $"{_requestId} {10}"),
                            new InlineKeyboardButton("✅", $"{_requestId} {-1}")
                        },
                });
    }

    private int Plus(int initNum, int addingNum)
    {
        return initNum * 10 + addingNum;
    }

    private int Minus(int initNum)
    {
        return initNum / 10;
    }

    private string DeleteLastDigit(string str)
    {
        var lastSymbol = str.Substring(str.Length - 1);

        if (int.TryParse(lastSymbol, out int num))        
            return str.Substring(0, str.Length - 1);   
        
        return str; 
    }
}