using System.Text;

class DepoCell : IComparable<DepoCell>
{
    public int Number { get; set; }
    public List<Box> RightSide { get; set; } = new List<Box>();
    public List<Box> LeftSide { get; set; } = new List<Box>();
    public List<Box> Center { get; set; } = new List<Box>();           
        
    public List<Box> GetBoxList(DepoCellSide side)
    {
        switch (side)
        {
            case DepoCellSide.Левая:
                return LeftSide;

            case DepoCellSide.Центр:
                return Center;

            case DepoCellSide.Правая:
                return RightSide;

            default:
                return null;
        }
    }

    public int CompareTo(DepoCell? other)
    {
        if (Number > other.Number)
            return 1;
        else if (Number < other.Number) 
            return -1; 
        else 
            return 0;
    }

    public string ToString(DepoCellSide side, bool numbered = false)
    {
        switch (side)
        {
            case DepoCellSide.Левая:
                return LeftSideToString(numbered);

            case DepoCellSide.Центр:
                return CenterToString(numbered);

            case DepoCellSide.Правая:
                return RightSideToString(numbered);
        }

        return "";
    }    

    public IEnumerable<Box> GetBoxes()
    {
        foreach (var box in LeftSide)
        {
            yield return box;
        }

        foreach (var box in Center)
        {
            yield return box;
        }

        foreach (var box in RightSide)
        {
            yield return box;
        }
    }

    private string LeftSideToString(bool isNumbered = false)
    {
        var text = new StringBuilder();
        text.Append($"<b>СХРОН № {Number}\nЛЕВАЯ  📦 {LeftSide.Count}</b>\n");

        if (LeftSide.Count == 0)
        {
            text.Append("- пусто -\n");
            return text.ToString();
        }
        else
        {
            for (int i = 0; i < LeftSide.Count; i++)
            {
                if (isNumbered)
                    text.Append($"[{i}]");

                text.Append($"{LeftSide[i].StatsToString()}");
            }
        }

        text.Append($"({Number}Л)");

        return text.ToString();
    }

    private string CenterToString(bool isNumbered = false)
    {
        var text = new StringBuilder();
        text.Append($"<b>СХРОН № {Number}\nЦЕНТР  📦 {Center.Count}</b>\n");

        if (Center.Count == 0)
        {
            text.Append("- пусто -\n");
            return text.ToString();
        }
        else
        {
            for (int i = 0; i < Center.Count; i++)
            {
                if (isNumbered)
                    text.Append($"[{i}]");

                text.Append($"{Center[i].StatsToString()}");
            }
        }

        text.Append($"({Number}Ц)");

        return text.ToString();
    }

    private string RightSideToString(bool isNumbered = false)
    {
        var text = new StringBuilder();
        text.Append($"<b>СХРОН № {Number}\nПРАВАЯ  📦 {RightSide.Count}</b>\n");

        if (RightSide.Count == 0)
        {
            text.Append("- пусто -\n");
            return text.ToString();
        }
        else
        {
            for (int i = 0; i < RightSide.Count; i++)
            {
                if (isNumbered)
                    text.Append($"[{i}]");

                text.Append($"{RightSide[i].StatsToString()}");
            }
        }

        text.Append($"({Number}П)");

        return text.ToString();
    }
}