class QueryHandler
{
    private RequestManager _requestManager;

    public void Set(RequestManager requestManager)
    {
        _requestManager = requestManager; 
    }    

    public async Task HandelQuery(string callbackData)
    {
        string[] dataPoints = callbackData.Split(' ');
        int requestId = int.Parse(dataPoints[0]);        
        
        if (int.TryParse(dataPoints[1], out int slideId))            
            await _requestManager.ProcessSlide(requestId, slideId);              
    }
}