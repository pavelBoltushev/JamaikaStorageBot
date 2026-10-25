using Newtonsoft.Json;

static class SaveLoader
{
    private static Dictionary<DataFile, string> _filePaths;

    static SaveLoader()
    {
        _filePaths = new()
        {
            [DataFile.Boxes] = "data/Boxes.txt",
            [DataFile.StorageLimit] = "data/limits/StorageLimit.txt",
            [DataFile.DepoLimit] = "data/limits/DepoLimit.txt",
            [DataFile.Reception] = "data/Reception.txt",
            [DataFile.StudioShipment] = "data/StudioShipment.txt",
            [DataFile.OrderShipment] = "data/OrderShipment.txt",
            [DataFile.Accounts] = "data/Accounts.txt",
            [DataFile.MessageUnitId] = "data/MessageUnitId.txt"
        };
    }

    public static void Load(Storage storage, AccountManager accountManager)
    {
        storage.Load(
            JsonConvert.DeserializeObject<List<Box>>(File.ReadAllText(_filePaths[DataFile.Boxes])),
            JsonConvert.DeserializeObject<List<Supply>>(File.ReadAllText(_filePaths[DataFile.Reception])),
            new StudioShipment(),
            //JsonConvert.DeserializeObject<StudioShipment>(File.ReadAllText(_filePaths[DataFile.StudioShipment])),
            JsonConvert.DeserializeObject<List<Order>>(File.ReadAllText(_filePaths[DataFile.OrderShipment])));

        accountManager.Load(JsonConvert.DeserializeObject<List<Account>>(File.ReadAllText(_filePaths[DataFile.Accounts])));

        MessageUnitIdCounter.Id = JsonConvert.DeserializeObject<int>(File.ReadAllText(_filePaths[DataFile.MessageUnitId]));
    }

    public static List<int> GetStorageLimit()
    {
        return JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(_filePaths[DataFile.StorageLimit]));
    }

    public static List<int> GetDepoLimit()
    {
        return JsonConvert.DeserializeObject<List<int>>(File.ReadAllText(_filePaths[DataFile.DepoLimit]));
    }

    public static void SaveMessageUnitId()
    {
        File.WriteAllText(_filePaths[DataFile.MessageUnitId], JsonConvert.SerializeObject(MessageUnitIdCounter.Id));
    }  

    public static void Save(List<Box> boxes)
    {
        File.WriteAllText(_filePaths[DataFile.Boxes], JsonConvert.SerializeObject(boxes));
    }

    public static void Save(List<Supply> supplies)
    {
        File.WriteAllText(_filePaths[DataFile.Reception], JsonConvert.SerializeObject(supplies));
    }

    public static void Save(StudioShipment studioShipment)
    {
        File.WriteAllText(_filePaths[DataFile.StudioShipment], JsonConvert.SerializeObject(studioShipment));
    }

    public static void Save(List<Order> orderShipment)
    {
        File.WriteAllText(_filePaths[DataFile.OrderShipment], JsonConvert.SerializeObject(orderShipment));
    }

    public static void Save(List<Account> accounts)
    {
        File.WriteAllText(_filePaths[DataFile.Accounts], JsonConvert.SerializeObject(accounts));
    }
}