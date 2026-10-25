using Telegram.Bot.Types.ReplyMarkups;

class SizeSlidePack
{
    private int _requestId;
    private Dictionary<int, BoxStatsSlide> _slides;

    public SizeSlidePack(int requestId)
    {
        _requestId = requestId;
        _slides = FillSlides();
    }

    public BoxStatsSlide this[int index]
    {
        get => _slides[index];
    }


    private Dictionary<int, BoxStatsSlide> FillSlides()
    {
        return new()
        {
            [0] = new(
                    text => text,
                    box => { },
                    () => SizeMarkup()),

            [37] = new(
                    text => text += "S\n",
                    box => box.Size = Size.S,
                    () => null),

            [38] = new(
                    text => text += "M\n",
                    box => box.Size = Size.M,
                    () => null),

            [39] = new(
                    text => text += "L\n",
                    box => box.Size = Size.L,
                    () => null),

            [40] = new(
                    text => text += "XL\n",
                    box => box.Size = Size.XL,
                    () => null),

            [41] = new(
                    text => text,
                    box => { },
                    () => FullSizeMarkup()),

            [42] = new(
                    text => text += "XS\n",
                    box => box.Size = Size.XS,
                    () => null),

            [43] = new(
                    text => text += "2XL\n",
                    box => box.Size = Size.XXL,
                    () => null),

            [44] = new(
                    text => text += "3XL\n",
                    box => box.Size = Size.XXXL,
                    () => null),

            [45] = new(
                    text => text += "4XL\n",
                    box => box.Size = Size.XXXXL,
                    () => null),

            [46] = new(
                    text => text += "5XL\n",
                    box => box.Size = Size.XXXXXL,
                    () => null)
        };
    }

    private InlineKeyboardMarkup SizeMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("S", $"{_requestId} {-37}"),
                            new InlineKeyboardButton("M", $"{_requestId} {-38}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("L", $"{_requestId} {-39}"),
                            new InlineKeyboardButton("XL", $"{_requestId} {-40}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Другие", $"{_requestId} {41}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup FullSizeMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("XS", $"{_requestId} {-42}"),
                            new InlineKeyboardButton("2XL", $"{_requestId} {-43}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("3XL", $"{_requestId} {-44}"),
                            new InlineKeyboardButton("4XL", $"{_requestId} {-45}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("5XL", $"{_requestId} {-46}")
                        }
                });
    }
}