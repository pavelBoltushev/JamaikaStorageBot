using System.Text;

class Account
{
    public long Id { get; init; }
    public string Name { get; set; }
    public List<Access> Accesses { get; set; } = new ();
    public List<Screen> Screens { get; set; } = new ();
    public List<Report> Reports { get; set; } = new();
    public bool Muted {  get; set; }

    public Account(long id)
    {
        Id = id;
        Name = id.ToString();
    }

    public override string ToString()
    {
        var text = new StringBuilder();
        text.Append($"<b>{(Muted ? "🔇" : "")}👤 {Name.ToUpper()}</b>\n\n");

        if (Accesses.Count == 0 && Screens.Count == 0 && Reports.Count == 0)
        {
            text.Append("🔒 Нет доступа");
            return text.ToString();
        }

        if (Accesses.Count != 0)
        {
            text.Append("<b>Команды:</b>\n");

            foreach (var access in Accesses)
            {
                text.Append($"    🕹 {access.ToString()}\n");
            }

            text.AppendLine();
        }

        if (Screens.Count != 0 || Reports.Count != 0)
        {
            text.Append($"<b>Подписки:</b>\n");

            foreach (var screen in Screens)
            {
                text.Append($"    📺 {screen.ToString()}\n");
            }

            foreach (var report in Reports)
            {
                text.Append($"    📖 {report.ToString()}\n");
            }
        }

        return text.ToString();
    }
}