class Adres
{
    public int Row { get; set; }
    public int Stillage { get; set; }
    public string Placement { get; set; } = string.Empty;
    public int DepoCellNumber { get; set; }
    public DepoCellSide DepoCellSide { get; set; }
    public bool isReception { get; set; }

    public void Reset()
    {
        Row = 0;
        Stillage = 0;
        Placement = string.Empty;
        DepoCellNumber = 0;
        DepoCellSide = 0;
        isReception = false;
    }

    public override string ToString()
    {
        if (isReception)
            return "~//~";
        else if (Row != 0)
            return $"{Row}.{Stillage}.{Placement}";
        else
            return $"#{DepoCellNumber}.{DepoCellSideToString()}";
    }    

    private string DepoCellSideToString()
    {
        switch (DepoCellSide)
        {
            case DepoCellSide.Левая:
                return "Л";
            case DepoCellSide.Центр:
                return "Ц";
            case DepoCellSide.Правая:
                return "П";
            default:
                return "";
        }
    }
}