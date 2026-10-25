using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

class AdresSlidePack
{
    private int _requestId;
    private Dictionary<int, AdresSlide> _slides;
    private List<int> _storageLimit;
    private List<int> _depoLimit;

    private int _currentInt;
    private bool _isDepo;
    private bool _incorrectAdres;
    private int _stage;

    public AdresSlidePack(int requestId)
    {
        _requestId = requestId;
        _slides = FillSlides();
        _storageLimit = SaveLoader.GetStorageLimit();
        _depoLimit = SaveLoader.GetDepoLimit();
    }

    public AdresSlide this[int index]
    {
        get => _slides[index];
    }

    public void Reset()
    {
        _stage = 0;
    }

    private Dictionary<int, AdresSlide> FillSlides()
    {
        return new()
        {
            [0] = new(
                text => text,
                adress => { },
                () => GetMarkup()),

            [1] = new(
                text => text += "<b>1</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 1);
                },
                () => GetMarkup()),

            [2] = new(
                text => text += "<b>2</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 2);
                },
                () => GetMarkup()),

            [3] = new(
                text => text += "<b>3</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 3);
                },
                () => GetMarkup()),

            [4] = new(
                text => text += "<b>4</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 4);
                },
                () => GetMarkup()),

            [5] = new(
                text => text += "<b>5</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 5);
                },
                () => GetMarkup()),

            [6] = new(
                text => text += "<b>6</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 6);
                },
                () => GetMarkup()),

            [7] = new(
                text => text += "<b>7</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 7);
                },
                () => GetMarkup()),

            [8] = new(
                text => text += "<b>8</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 8);
                },
                () => GetMarkup()),

            [9] = new(
                text => text += "<b>9</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 9);
                },
                () => GetMarkup()),

            [10] = new(
                text => text += "<b>0</b>",
                adress =>
                {
                    _currentInt = Plus(_currentInt, 0);
                },
                () => GetMarkup()),

            [11] = new(
                text => DeleteLastLine(text) + "\n>Адрес: ",
                adress =>
                {
                    adress.Reset();
                    _stage = 0;
                },
                () => GetMarkup()),

            [12] = new(
                text => ProcessCorrectionText(text),
                adress => ProcessNumber(adress),
                () => GetMarkup()),

            [13] = new(
                text => text += "<b>1.</b>",
                adress =>
                {
                    _currentInt = 0;
                    adress.Row = 1;
                    _isDepo = false;
                    _stage = 1;
                },
                () => GetMarkup()),

            [14] = new(
                text => text += "<b>2.</b>",
                adress =>
                {
                    _currentInt = 0;
                    adress.Row = 2;
                    _isDepo = false;
                    _stage = 1;
                },
                () => GetMarkup()),

            [16] = new(
                text => text += "<b>#</b>",
                adress =>
                {
                    _currentInt = 0;
                    _isDepo = true;
                    _stage = 1;
                },
                () => GetMarkup()),

            [17] = new(
                text => text += "<b>Е</b>",
                adress =>
                {
                    adress.Placement = "Е";
                    _stage = 4;
                },
                () => GetMarkup()),

            [18] = new(
                text => text += "<b>Д</b>",
                adress =>
                {
                    adress.Placement = "Д";
                    _stage = 4;
                },
                () => GetMarkup()),

            [19] = new(
                text => text += "<b>Г</b>",
                adress =>
                {
                    adress.Placement = "Г";
                    _stage = 4;
                },
                () => GetMarkup()),

            [20] = new(
                text => text += "<b>В</b>",
                adress =>
                {
                    adress.Placement = "В";
                    _stage = 4;
                },
                () => GetMarkup()),

            [21] = new(
                text => text += "<b>Б</b>",
                adress =>
                {
                    adress.Placement = "Б";
                    _stage = 4;
                },
                () => GetMarkup()),

            [22] = new(
                text => text += "<b>А</b>",
                adress =>
                {
                    adress.Placement = "А";
                    _stage = 4;
                },
                () => GetMarkup()),

            [23] = new(
                text => text += "<b>Л</b>",
                adress =>
                {
                    adress.DepoCellSide = DepoCellSide.Левая;
                    _stage = 4;
                },
                () => GetMarkup()),

            [24] = new(
                text => text += "<b>Ц</b>",
                adress =>
                {
                    adress.DepoCellSide = DepoCellSide.Центр;
                    _stage = 4;
                },
                () => GetMarkup()),

            [25] = new(
                text => text += "<b>П</b>",
                adress =>
                {
                    adress.DepoCellSide = DepoCellSide.Правая;
                    _stage = 4;
                },
                () => GetMarkup())
        };
    }

    private InlineKeyboardMarkup GetMarkup()
    {
        switch (_stage)
        {
            case 0:
                return RowDepoMarkup();

            case 1:
                return NumberMarkup();

            case 2:
                return Letter6Markup();

            case 3:
                return DepoCellSideMarkup();

            case 4:
                return ExecutionMarkup();

            default:
                return null;
        }
    }

    private InlineKeyboardMarkup RowDepoMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("1", $"{_requestId} {13}"),
                            new InlineKeyboardButton("2", $"{_requestId} {14}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("#", $"{_requestId} {16}")
                        }
                });
    }

    private InlineKeyboardMarkup NumberMarkup()
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
                            new InlineKeyboardButton("✅", $"{_requestId} {12}")
                        },
                });
    }

    private InlineKeyboardMarkup Letter6Markup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Е", $"{_requestId} {17}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Д", $"{_requestId} {18}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Г", $"{_requestId} {19}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("В", $"{_requestId} {20}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Б", $"{_requestId} {21}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("А", $"{_requestId} {22}")
                        }
                });
    }

    private InlineKeyboardMarkup DepoCellSideMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Левая", $"{_requestId} {23}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Центр", $"{_requestId} {24}")
                        },
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("Правая", $"{_requestId} {25}")
                        }
                });
    }

    private InlineKeyboardMarkup ExecutionMarkup()
    {
        return new(
                new List<List<InlineKeyboardButton>>
                {
                        new List<InlineKeyboardButton>
                        {
                            new InlineKeyboardButton("◀️", $"{_requestId} {11}"),
                            new InlineKeyboardButton("✅", $"{_requestId} {-1}")
                        }
                });
    }

    private int Plus(int initNum, int addingNum)
    {
        return initNum * 10 + addingNum;
    }      
    
    private string DeleteLastLine(string input)
    {
        List<string> lines = input.Split('\n').ToList(); 
        lines.RemoveAt(lines.Count - 1);
        return string.Join('\n', lines);
    }

    private void ProcessNumber(Adres adres)
    {
        if (_isDepo)
        {
            adres.DepoCellNumber = _currentInt;
            _stage = 3;
        }
        else
        {
            adres.Stillage = _currentInt;
            _stage = 2;
        }

        if (!AdresCorrect(adres))        
        {
            _incorrectAdres = true;
            adres.Reset();
            _stage = 0;
        }
    }

    private string ProcessCorrectionText(string text)
    {
        string addedText = "";

        if (_incorrectAdres)
            addedText = "❗️\n>Адрес: ";
        else
            addedText = "<b>.</b>";

        _incorrectAdres = false;
        return text += addedText;
    }

    private bool AdresCorrect(Adres adres)
    {
        bool correct = false; 

        if (adres.Row > 0 && adres.Row <= _storageLimit.Count &&
            adres.Stillage > 0 && adres.Stillage <= _storageLimit[adres.Row - 1])
            correct = true;
            
        if (_depoLimit.Contains(adres.DepoCellNumber))
            correct = true;

        return correct;
    }
}