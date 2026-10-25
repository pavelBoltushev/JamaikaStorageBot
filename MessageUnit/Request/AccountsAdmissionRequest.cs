using Telegram.Bot.Types.ReplyMarkups;

class AccountsAdmissionRequest : Request
{
    private List<Account> _accounts = new ();

    private int _accountIndex;
    private int _stage;

    private Account CurrentAccount => _accounts [_accountIndex];

    public override async Task InitRequest()
    {
        _accounts = _accountManager.GetAccounts();
        await CreateMessage(CurrentAccount.ToString(), OperationMarkup());
    }

    public override async Task Execute(bool processing, int slideId = 0)
    {
        switch (_stage)
        {
            case 0:
                if (processing)
                {
                    await ProcessOperationSlide(slideId);
                }
                break;

            case 1:
                if (processing)
                {
                    await ProcessAccountManagementSlide(slideId);
                }
                break;
        }
    }

    public override async Task EnterText(string text)
    {
        CurrentAccount.Name = text;
        await ChangeMessage(CurrentAccount.ToString(), AccountManagementMarkup());
        TextAwaiting = false;
    }

    private InlineKeyboardMarkup OperationMarkup()
    {
        var buttonsList = new List<List<InlineKeyboardButton>>();

        buttonsList.Add(new() { new("⬅️", $"{Id} {1}"), new("➡️", $"{Id} {2}") });
        buttonsList.Add(new() { new("➡️👤", $"{Id} {3}") });
        buttonsList.Add(new() { new($"{(CurrentAccount.Muted ? "🔈" : "🔇")}", $"{Id} {4}") });
        buttonsList.Add(new() { new("❌ Закрыть", $"{Id} {5}") });

        return new InlineKeyboardMarkup(buttonsList);
    }

    private async Task ProcessOperationSlide(int slideId)
    {
        switch (slideId)
        {
            case 1:
                _accountIndex = _accountIndex == 0 ? _accounts.Count - 1 : --_accountIndex;
                break;

            case 2:
                _accountIndex = _accountIndex == _accounts.Count - 1 ? 0 : ++_accountIndex;
                break;

            case 3:
                await ChangeMessage(CurrentAccount.ToString(), AccountManagementMarkup());
                _stage = 1;
                return;

            case 4:
                CurrentAccount.Muted = !CurrentAccount.Muted;
                SaveLoader.Save(_accounts);
                break;

            case 5:
                await CloseRequest();
                return;
        }

        await ChangeMessage(CurrentAccount.ToString(), OperationMarkup());
    }

    private InlineKeyboardMarkup AccountManagementMarkup()
    {
        var buttonsList = new List<List<InlineKeyboardButton>>();
        buttonsList.Add(new() { new("💬 Имя", $"{Id} {-1}") });

        foreach (var screen in Enum.GetValues<Screen>())        
        {
            string buttonTitle = CurrentAccount.Screens.Contains(screen) ?
                                    $"➖📺 {screen.ToString()}" :
                                    $"➕📺 {screen.ToString()}";

            buttonsList.Add(new() { new(buttonTitle, $"{Id} {(int)screen}") });
        }

        foreach (var report in Enum.GetValues<Report>())
        {
            string buttonTitle = CurrentAccount.Reports.Contains(report) ?
                                    $"➖📖 {report.ToString()}" :
                                    $"➕📖 {report.ToString()}";

            buttonsList.Add(new() { new(buttonTitle, $"{Id} {(int)report + 10}") });
        }

        buttonsList.Add(new() { new("◀️ Назад", $"{Id} {-2}") });

        return new InlineKeyboardMarkup( buttonsList);
    }

    private async Task ProcessAccountManagementSlide(int slideId)
    {
        if(slideId == -1)
        {
            await ChangeMessage(CurrentAccount.ToString() + "💬⤵️", null);
            TextAwaiting = true;
        }
        else if (slideId == -2)
        {
            SaveLoader.Save(_accounts);
            await ChangeMessage(CurrentAccount.ToString(), OperationMarkup());
            _stage = 0;
        }
        else if (slideId < 10)
        {
            if (CurrentAccount.Screens.Contains((Screen)slideId))            
                CurrentAccount.Screens.Remove((Screen)slideId);
            else
                CurrentAccount.Screens.Add((Screen)slideId);

            await ChangeMessage(CurrentAccount.ToString(), AccountManagementMarkup());
        }
        else
        {
            slideId -= 10;

            if (CurrentAccount.Reports.Contains((Report)slideId))
                CurrentAccount.Reports.Remove((Report)slideId);
            else
                CurrentAccount.Reports.Add((Report)slideId);

            await ChangeMessage(CurrentAccount.ToString(), AccountManagementMarkup());
        }
    }
}