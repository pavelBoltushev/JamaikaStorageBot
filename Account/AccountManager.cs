class AccountManager
{
    public readonly long AdminId = 553949259;
    private List<Account> _accounts = new List<Account>(); 

    public void Load(List<Account> accounts)
    {
        _accounts = accounts;
    }

    public List<Account> GetAccounts()
    {
        return _accounts;
    }

    public void AddAccount(long senderId)
    {
        foreach (var account in _accounts)
        {
            if (account.Id == senderId)                          
                return;                     
        }        
        
        _accounts.Add(new Account(senderId));        
    }

    public bool TryGetAccount(long senderId, out Account account)
    {      
        foreach (Account currentAccount in _accounts)
        {
            if (currentAccount.Id == senderId)
            {
                account = currentAccount;
                return true;
            }                
        }

        account = null;
        return false;
    }  

    public List<long> GetSenderIds(Screen screen)
    {
        var senderIds = new List<long>();

        foreach (var account in _accounts)
        {
            if (account.Screens.Contains(screen) && !account.Muted)            
                senderIds.Add(account.Id);            
        }

        return senderIds;
    }

    public List<long> GetSenderIds(Report report)
    {
        var senderIds = new List<long>();

        foreach (var account in _accounts)
        {
            if (account.Reports.Contains(report) && !account.Muted)            
                senderIds.Add(account.Id);            
        }

        return senderIds;
    }
}