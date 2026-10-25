class Studio
{
    public StudioAdress Adress { get; set; }
    public int Count { get; set; }
    public Status Status { get; set; }    

    //public override string ToString()
    //{
    //    return $"<b>{Adress.ToString().ToUpper()} {CountToString()}</b>\n\n" +
    //        $"{StatusToString()}";
    //}

    public override string ToString()
    {
        return $"<b>{Adress.ToString().ToUpper()} {CountToString()}</b>\n" +
            $"       {StatusToString()}\n";
    }

    private string CountToString()
    {
        if (Count == 0)
            return "🏷";
        else
            return $"📦 - {Count}";
    }

    private string StatusToString()
    {
        switch (Status)
        {
            case Status.Сборка:
                return "🫳 Сборка";                

            case Status.Скан:
                return "🔡 Скан";

            case Status.Отгрузка:
                return "🚚 Готов к отгрузке";

            case Status.Выполнен:
                return "✅ Отгружено";

            default:
                return "";
        }
    }
}