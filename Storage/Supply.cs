using System.Text;

class Supply : IComparable<Supply>
{    
    public int Number {  get; set; }
    public Box Stats { get; set; } = new Box();
    public bool IsUnpacked { get; set; }
    public List<Box> Boxes { get; set; } = new List<Box>();

    public int CompareTo(Supply? other)
    {
        if (Number > other.Number)
            return 1;
        else if (Number < other.Number) 
            return -1; 
        else 
            return 0;
    }

    public override string ToString()
    {
        var text = new StringBuilder();
        
        if (IsUnpacked)
        {
            text.Append($"<b>ПОСТАВКА № {Number}</b>  📦 - {Boxes.Count}\n\n");

            foreach (Box box in Boxes)
                text.Append($"{box.StatsToString()}");
        }
        else
        {
            text.Append($"<b>ПОСТАВКА № {Number}</b>\n");
            text.Append(StatsToString());
        }
        
        return text.ToString();
    }

    public string StatsToString()
    {
        return $"{Stats.Type.ToString().ToLower()} " +
            $"{Stats.ModelToString()} " +
            $"{Stats.TextileToString()}\n" +
            $"{Stats.Color.ToString().ToLower()} " +
            $"{(Stats.Count == 0 ? "" : $"- {Stats.Count}")}\n";
    }

    public string NumberedToString()
    {
        var text = new StringBuilder();
        text.Append($"<b>ПОСТАВКА № {Number}</b>\n\n");

        for (int i = 0; i < Boxes.Count; i++)
        {
            text.Append($"[{i}]{Boxes[i].StatsToString()}");
        }

        return text.ToString();
    }

    public string RemoveMarkedToString(Box boxToRemove)
    {
        var text = new StringBuilder();
        text.Append($"<b>ПОСТАВКА № {Number}</b>\n\n");

        foreach (var box in Boxes)
        {
            if (boxToRemove == box)
                text.Append($"➖{box.StatsToString()}");
            else
                text.Append(box.StatsToString());
        }

        return text.ToString();
    }
}