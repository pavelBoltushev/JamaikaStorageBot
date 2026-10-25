class Order
{
    public int Number {  get; set; }
    public int Count {  get; set; }
    public string Sign { get; set; } = "-";
    public Status Status { get; set; }
    public ScreenUnit? ScreenUnit { get; set; } = null;

    public override string ToString()
    {
        return $"<b>№{Number} {CountToString()}\n{Sign}</b>\n{StatusToString()}\n";
    }

    public string MarkedToString()
    {
        return $"<b>♦️{Number} {CountToString()}\n{Sign}</b>\n{StatusToString()}\n";
    }

    public string SignAwaitToString()
    {
        return $"<b>№{Number} {CountToString()}</b>\n" +
            $"💬⤵️";
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
            case Status.Отгрузка:
                return "🚚 Готов к отгрузке";

            case Status.Выполнен:
                return "✅ Отгружено";

            default:
                return "";
        }
    }
}