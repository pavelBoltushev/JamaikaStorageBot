class Box
{
    public Adres Adress { get; set; } = new Adres();
    public Type Type { get; set; }
    public Model Model { get; set; }
    public Textile Textile { get; set; }
    public Color Color { get; set; }
    public Size Size { get; set; }
    public int Count { get; set; }
    public int SupplyNumber { get; set; }

    public bool Match(Box box)
    {
        if (Type == box.Type&&
            Model == box.Model &&
            Textile == box.Textile &&
            Color == box.Color &&
            Size == box.Size)
             return true; 

        return false;
    }

    public void Reset()
    {
        Type = 0;
        Model = 0;
        Textile = 0;
        Color = 0;
        Size = 0;
    }

    public void SetSupplyStats(Box supplyStats)
    {
        Type = supplyStats.Type;
        Model = supplyStats.Model;
        Textile = supplyStats.Textile;
        Color = supplyStats.Color;
    } 

    public string FullToString()
    {
        return $"{StatsToString()}<b>{Adress.ToString()}</b>\n";
    }

    public string StatsToString()
    {
        return $"📦 {(SupplyNumber == 0 ? "" : $"({SupplyNumber})")}\n" +
            $"{Type.ToString().ToLower()} " +
            $"{ModelToString()}" +
            $"{TextileToString()}\n" +
            $"{Color.ToString().ToLower()} " +
            $"{SizeToString(Size)} " +
            $"{(Count == 0 ? "" : $"- {Count}")}\n";
    }

    public string StatisticsTitleToString()
    {
        var title = $"{Type.ToString().ToLower()} " +
            $"{ModelToString()}" +
            $"{TextileToString()}\n";

        return title.ToUpper();
    }

    public string ReviewToSting()
    {
        return $"{Type.ToString().ToLower()} " +
            $"{ModelToString()}" +
            $"{TextileToString()}\n" +
            $"{Color.ToString().ToLower()} " +
            $"{SizeToString(Size)} " +
            $"{(Count == 0 ? "" : $"- {Count}")} {(SupplyNumber == 0 ? "" : $"({SupplyNumber})")}\n" +
            $"{Adress.ToString()}\n\n";
    }

    public string ModelToString()
    {
        switch (Model)
        {
            case Model.None:
                return "";

            case Model.Оверсайз_26:
                return "оверсайз-26";            

            case Model.SklepsterNW:
                return "sklepsterNW ";

            case Model.Карман:
                return "с карманом ";

            default:
                return Model.ToString().ToLower() + " ";
        }
    }

    public string TextileToString()
    {
        switch (Textile)
        {
            case Textile.None:
                return "";

            case Textile.Гр230:
                return "230";

            case Textile.Гр220:
                return "220";

            case Textile.Гр180:
                return "180";

            default:
                return Textile.ToString().ToLower();
        }
    }

    public static string SizeToString(Size size)
    {
        switch (size)
        {            
            case Size.XXL:
                return "2XL";

            case Size.XXXL:
                return "3XL";

            case Size.XXXXL:
                return "4XL";

            case Size.XXXXXL:
                return "5XL";

            default:
                return size.ToString();
        }
    }
}