using Telegram.Bot.Types.ReplyMarkups;

class SupplyStatsSlidePack
{
    private int _requestId;
    private Dictionary<int, BoxStatsSlide> _slides;

    public SupplyStatsSlidePack(int requestId)
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
                    () => ColorMarkup()),

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
                    () => ColorMarkup()),

            [7] = new(
                    text => text += "Оверсайз\n",
                    box => box.Model = Model.Оверсайз,
                    () => OversizeTextileMarkup()),

            [8] = new(
                    text => text += "SKLEPSTER\n",
                    box => box.Model = Model.SKLEPSTER,
                    () => ColorMarkup()),

            [9] = new(
                    text => text += "Унисекс\n",
                    box => box.Model = Model.Унисекс,
                    () => ColorMarkup()),

            [10] = new(
                    text => text += "Стандарт\n",
                    box => box.Textile = Textile.Стандарт,
                    () => ColorMarkup()),

            [11] = new(
                    text => text += "Премиум\n",
                    box => box.Textile = Textile.Премиум,
                    () => ColorMarkup()),

            [12] = new(
                    text => text += "Черный\n",
                    box => box.Color = Color.Черный,
                    () => null),

            [13] = new(
                    text => text += "Белый\n",
                    box => box.Color = Color.Белый,
                    () => null),

            [14] = new(
                    text => text,
                    box => { },
                    () => FullColorMarkup()),

            [15] = new(
                    text => text += "Бежевый\n",
                    box => box.Color = Color.Бежевый,
                    () => null),

            [17] = new(
                    text => text += "Вар черная\n",
                    box => box.Color = Color.Варенка_Черный,
                    () => null),

            [18] = new(
                    text => text += "Вар красная\n",
                    box => box.Color = Color.Варенка_Красный,
                    () => null),

            [19] = new(
                    text => text += "Вар розовая\n",
                    box => box.Color = Color.Варенка_Розовый,
                    () => null),

            [20] = new(
                    text => text += "Вар хаки\n",
                    box => box.Color = Color.Варенка_Хаки,
                    () => null),

            [21] = new(
                    text => text += "Голубой\n",
                    box => box.Color = Color.Голубой,
                    () => null),

            [22] = new(
                    text => text += "Графит 30-02\n",
                    box => box.Color = Color.Графит_3002,
                    () => null),

            [23] = new(
                    text => text += "Графит 40-04\n",
                    box => box.Color = Color.Графит_4004,
                    () => null),

            [24] = new(
                    text => text += "Графит 468\n",
                    box => box.Color = Color.Графит_468,
                    () => null),

            [25] = new(
                    text => text += "Графит 479\n",
                    box => box.Color = Color.Графит_479,
                    () => null),

            [26] = new(
                    text => text += "Желтый\n",
                    box => box.Color = Color.Желтый,
                    () => null),

            [27] = new(
                    text => text += "Зеленый\n",
                    box => box.Color = Color.Зеленый,
                    () => null),

            [28] = new(
                    text => text += "Индиго\n",
                    box => box.Color = Color.Индиго,
                    () => null),

            [29] = new(
                    text => text += "Красный\n",
                    box => box.Color = Color.Красный,
                    () => null),

            [30] = new(
                    text => text += "Лиловый\n",
                    box => box.Color = Color.Лиловый,
                    () => null),

            [31] = new(
                    text => text += "Меланж\n",
                    box => box.Color = Color.Меланж,
                    () => null),

            [32] = new(
                    text => text += "Полынь\n",
                    box => box.Color = Color.Полынь,
                    () => null),

            [33] = new(
                    text => text += "Св. серый\n",
                    box => box.Color = Color.Св_серый,
                    () => null),

            [34] = new(
                    text => text += "Синий\n",
                    box => box.Color = Color.Синий,
                    () => null),

            [35] = new(
                    text => text += "Т. синий\n",
                    box => box.Color = Color.Т_синий,
                    () => null),

            [36] = new(
                    text => text += "Хаки\n",
                    box => box.Color = Color.Хаки,
                    () => null),

            [47] = new(
                    text => text += "230 гр\n",
                    box => box.Textile = Textile.Гр230,
                    () => ColorMarkup()),

            [48] = new(
                    text => text += "220 гр\n",
                    box => box.Textile = Textile.Гр220,
                    () => ColorMarkup()),

            [49] = new(
                    text => text += "180 гр\n",
                    box => box.Textile = Textile.Гр180,
                    () => ColorMarkup()),

            [50] = new(
                    text => text += "Оверсайз\n",
                    box => box.Model = Model.Оверсайз,
                    () => ColorMarkup()),

            [51] = new(
                    text => text += "Бордо 528\n",
                    box => box.Color = Color.Бордо_528,
                    () => null),

            [52] = new(
                    text => text += "Бордо 550\n",
                    box => box.Color = Color.Бордо_550,
                    () => null),

            [53] = new(
                    text => text += "SklepsterNW\n",
                    box => box.Model = Model.SklepsterNW,
                    () => ColorMarkup()),

            [54] = new(
                    text => text += "с карманом\n",
                    box => box.Model = Model.Карман,
                    () => ColorMarkup()),

            [55] = new(
                    text => text += "Оверсайз-26\n",
                    box => box.Model = Model.Оверсайз_26,
                    () => ColorMarkup())
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
                            new InlineKeyboardButton("Лонгслив", $"{_requestId} {2}")
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
                            new InlineKeyboardButton("Оверсайз-26", $"{_requestId} {55}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Sklepster", $"{_requestId} {8}"),
                            new InlineKeyboardButton("SklepsterNW", $"{_requestId} {53}")
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
                            new InlineKeyboardButton("Оверсайз", $"{_requestId} {50}"),
                            new InlineKeyboardButton("Унисекс", $"{_requestId} {9}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Sklepster", $"{_requestId} {8}"),
                            new InlineKeyboardButton("с карманом", $"{_requestId} {54}")
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
                            new InlineKeyboardButton("Оверсайз", $"{_requestId} {50}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Унисекс", $"{_requestId} {9}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Женский", $"{_requestId} {6}")
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
                            new InlineKeyboardButton("Стандарт", $"{_requestId} {10}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Премиум", $"{_requestId} {11}")
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
                            new InlineKeyboardButton("230", $"{_requestId} {47}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("220", $"{_requestId} {48}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("180", $"{_requestId} {49}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup ColorMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Черный", $"{_requestId} {-12}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Белый", $"{_requestId} {-13}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Другие", $"{_requestId} {14}")
                        }
                }
                );
    }

    private InlineKeyboardMarkup FullColorMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Бежевый", $"{_requestId} {-15}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Бордо 528", $"{_requestId} {-51}"),
                            new InlineKeyboardButton("Бордо 550", $"{_requestId} {-52}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Вар черная", $"{_requestId} {-17}"),
                            new InlineKeyboardButton("Вар красная", $"{_requestId} {-18}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Вар розовая", $"{_requestId} {-19}"),
                            new InlineKeyboardButton("Вар хаки", $"{_requestId} {-20}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Голубой", $"{_requestId} {-21}"),
                            new InlineKeyboardButton("Графит 30-02", $"{_requestId} {22}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Графит 40-04", $"{_requestId} {-23}"),
                            new InlineKeyboardButton("Графит 468", $"{_requestId} {-24}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Графит 479", $"{_requestId} {-25}"),
                            new InlineKeyboardButton("Желтый", $"{_requestId} {-26}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Зеленый", $"{_requestId} {-27}"),
                            new InlineKeyboardButton("Индиго", $"{_requestId} {-28}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Красный", $"{_requestId} {-29}"),
                            new InlineKeyboardButton("Лиловый", $"{_requestId} {-30}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Меланж", $"{_requestId} {-31}"),
                            new InlineKeyboardButton("Полынь", $"{_requestId} {-32}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Св. серый", $"{_requestId} {-33}"),
                            new InlineKeyboardButton("Синий", $"{_requestId} {-34}"),
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Т. синий", $"{_requestId} {-35}"),
                            new InlineKeyboardButton("Хаки", $"{_requestId} {-36}"),
                        }
                }
                );
    }
}