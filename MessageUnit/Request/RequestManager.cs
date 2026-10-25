class RequestManager
{    
    private Storage _storage;
    private Terminal _terminal;
    private AccountManager _accountManager;

    private Dictionary<int, Request> _requests = new Dictionary<int, Request>();    

    public void Set(Storage storage, Terminal terminal, AccountManager accountManager)
    {
        _storage = storage;
        _terminal = terminal;
        _accountManager = accountManager;
    }    
    
    public async Task Init<T>(long senderId, int commandMessageId) where T : Request, new()
    {
        var request = new T();
        request.Set(senderId, commandMessageId, _terminal, _storage, _accountManager);
        request.RequestFinished += RemoveRequest;
        _requests.Add(request.Id, request);
        await request.InitRequest();
    }

    public async Task ProcessSlide(int requestId, int slideId)
    {
        await _requests[requestId].ProcessSlide(slideId);
    }

    public async Task ProcessStage(int requestId)
    {
        await _requests[requestId].ProcessStage();
    }

    public async Task EnterText(int requestId, string text)
    {
        await _requests[requestId].EnterText(text);
    }

    public bool IsTextAwaiting(long senderId, out int requestId)
    {
        foreach (var pair in _requests)
        {
            var request = pair.Value;

            if (request.MatchSenderId(senderId) && request.TextAwaiting)
            {
                requestId = pair.Key;
                return true;
            }
        }

        requestId = 0;
        return false;
    }

    private void RemoveRequest(int requestId)
    {
        _requests.Remove(requestId);
    }
}