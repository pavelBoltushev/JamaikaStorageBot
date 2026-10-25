static class MessageUnitIdCounter
{
    public static int Id {  get; set; }

    public static int GetId()
    {
        return Id++;
    }    
}