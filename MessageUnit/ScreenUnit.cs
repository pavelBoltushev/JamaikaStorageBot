class ScreenUnit
{
    public Screen Screen { get; set; }
    public List<(long senderId, int messageId)> Ids { get; set; } = new List<(long senderId, int messageId)>();
    public string MessageText { get; set; } = "";

    public string TextView => MessageText;
}