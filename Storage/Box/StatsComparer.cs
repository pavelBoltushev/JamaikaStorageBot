class StatsComparer : IComparer<Box>
{
    public int Compare(Box? x, Box? y)
    {
        if (x?.Type > y?.Type)
            return 1;
        else if (x?.Type < y?.Type)
            return -1;
        else if (x?.Model > y?.Model)
            return 1;
        else if (x?.Model < y?.Model)
            return -1;
        else if (x?.Textile > y?.Textile)
            return 1;
        else if (x?.Textile < y?.Textile)
            return -1;
        else if (x?.Color > y?.Color)
            return 1;
        else if (x?.Color < y?.Color)
            return -1;
        else if (x?.Size > y?.Size)
            return 1;
        else if (x?.Size < y?.Size)
            return -1;
        else
            return 0;
    }
}