class StillagePlacementComparer : IComparer<Box>
{
    public int Compare(Box? x, Box? y)
    {
        if (x?.Adress.Placement.CompareTo(y?.Adress.Placement) < 0)
            return 1;
        else if (x?.Adress.Placement.CompareTo(y?.Adress.Placement) > 0)
            return -1;
        else
            return 0;
    }
}