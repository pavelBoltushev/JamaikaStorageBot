using Telegram.Bot.Types.ReplyMarkups;

class CoreStatsSlidePack
{
    private int _requestId;
    private Dictionary<int, BoxStatsSlide> _slides;

    public CoreStatsSlidePack(int requestId)
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
                    () => InitTypeMarkup()),

            [1] = new(
                    text => text += "Футболка\n",
                    box => box.Type = Type.Футболка,
                    () => TShirtModelMarkup()),

            [2] = new(
                    text => text += "Лонгслив SKLEP\n",
                    box =>
                    {
                        box.Type = Type.Лонгслив;
                        box.Model = Model.SKLEPSTER;
                    },
                    () => null),

            [3] = new(
                    text => text += "Свитшот\n",
                    box => box.Type = Type.Свитшот,
                    () => SweatshotModelMarkup()),

            [4] = new(
                    text => text += "Толстовка\n",
                    box => box.Type = Type.Толстовка,
                    () => HoodyModelMarkup()),

            [5] = new(
                    text => text += "Мужской\n",
                    box => box.Model = Model.Мужской,
                    () => TShirtTextileMarkup()),

            [6] = new(
                    text => text += "Женский\n",
                    box => box.Model = Model.Женский,
                    () => null),

            [7] = new(
                    text => text += "Оверсайз\n",
                    box => box.Model = Model.Оверсайз,
                    () => OversizeTextileMarkup()),

            [8] = new(
                    text => text += "SKLEPSTER\n",
                    box => box.Model = Model.SKLEPSTER,
                    () => null),

            [9] = new(
                    text => text += "Унисекс\n",
                    box => box.Model = Model.Унисекс,
                    () => null),

            [10] = new(
                    text => text += "Стандарт\n",
                    box => box.Textile = Textile.Стандарт,
                    () => null),

            [11] = new(
                    text => text += "Премиум\n",
                    box => box.Textile = Textile.Премиум,
                    () => null),

            [47] = new(
                    text => text += "230 гр\n",
                    box => box.Textile = Textile.Гр230,
                    () => null),

            [48] = new(
                    text => text += "220 гр\n",
                    box => box.Textile = Textile.Гр220,
                    () => null),

            [49] = new(
                    text => text += "180 гр\n",
                    box => box.Textile = Textile.Гр180,
                    () => null),

            [50] = new(
                    text => text += "Оверсайз\n",
                    box => box.Model = Model.Оверсайз,
                    () => null),

            [53] = new(
                    text => text += "SklepsterNW\n",
                    box => box.Model = Model.SklepsterNW,
                    () => null),

            [54] = new(
                    text => text += "с карманом\n",
                    box => box.Model = Model.Карман,
                    () => null),

            [55] = new(
                    text => text += "Оверсайз-26\n",
                    box => box.Model = Model.Оверсайз_26,
                    () => null)
        };
    }

    private InlineKeyboardMarkup InitTypeMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Футболка", $"{_requestId} {1}"),
                            new InlineKeyboardButton("Лонгслив", $"{_requestId} {-2}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Свитшот", $"{_requestId} {3}"),
                            new InlineKeyboardButton("Толстовка", $"{_requestId} {4}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup TShirtModelMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Мужской", $"{_requestId} {5}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Оверсайз", $"{_requestId} {7}"),
                            new InlineKeyboardButton("Оверсайз-26", $"{_requestId} {-55}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Sklepster", $"{_requestId} {-8}"),
                            new InlineKeyboardButton("SklepsterNW", $"{_requestId} {-53}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup HoodyModelMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Оверсайз", $"{_requestId} {-50}"),
                            new InlineKeyboardButton("Унисекс", $"{_requestId} {-9}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Sklepster", $"{_requestId} {-8}"),
                            new InlineKeyboardButton("с карманом", $"{_requestId} {-54}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup SweatshotModelMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Оверсайз", $"{_requestId} {-50}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Унисекс", $"{_requestId} {-9}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Женский", $"{_requestId} {-6}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup TShirtTextileMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Стандарт", $"{_requestId} {-10}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Премиум", $"{_requestId} {-11}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup OversizeTextileMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("230", $"{_requestId} {-47}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("220", $"{_requestId} {-48}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("180", $"{_requestId} {-49}")
                        }
                }
                );
    }      
}