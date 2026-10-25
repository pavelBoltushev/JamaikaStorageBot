class Storage
{       
    private List<Box> _boxes { get; set; } = new ();
    private List<Supply> _reception { get; set; } = new ();
    private StudioShipment _studioShipment { get; set; } = new ();
    private List<Order> _orderShipment { get; set; } = new ();

    public void Load(List<Box> boxes, List<Supply> reception, StudioShipment studioShipment, List<Order> orderShipment)
    {
        _boxes = boxes;
        _reception = reception;
        _studioShipment = studioShipment;
        _orderShipment = orderShipment;
    }

    public void PutBox(Box box)
    {
        _boxes.Add(box);
        SaveLoader.Save(_boxes);
    }

    public void RemoveBox(Box box)
    {
        foreach (var currentBox in _boxes)
        {
            if (currentBox == box)
            {
                _boxes.Remove(currentBox);
                SaveLoader.Save(_boxes);
                return;
            }
        }

        foreach (var supply in _reception)
            foreach (var currentBox in supply.Boxes)
            {
                if (currentBox == box)
                {
                    supply.Boxes.Remove(currentBox);
                    SaveLoader.Save(_reception);
                    return;
                }
            }
    }

    public IEnumerable<Box> GetAllBoxes()
    {               
        foreach (var box in _boxes)
        {
            yield return box;
        }

        foreach(var supply in _reception)        
            foreach (var box in supply.Boxes)
            {
                yield return box;
            }        
    }

    public IEnumerable<Box> ExtractBy(Type type, Model model, Textile textile)
    {
        foreach (var box in GetAllBoxes())
        {
            if (box.Type == type && box.Model == model && box.Textile == textile)
                yield return box;
        }
    }

    public List<Supply> GetReception()
    {
        return _reception;
    }

    public StudioShipment GetStudioShipment()
    {
        return _studioShipment;
    }

    public List<Order> GetOrderShipment()
    {
        return _orderShipment;
    }

    public List<List<List<Box>>> GetStorageList()
    {
        SaveLoader.Save(_boxes);
        var sortedBoxesList = new List<List<List<Box>>>();
        var storageLimit = SaveLoader.GetStorageLimit();

        for (int i = 0; i < storageLimit.Count; i++)
        {
            sortedBoxesList.Add(new List<List<Box>>());

            for (int j = 0; j < storageLimit[i]; j++)
            {
                sortedBoxesList[i].Add(new List<Box>());
            }
        }

        foreach (var box in _boxes)
        {
            if(box.Adress.Row != 0)
            {
                int rowIndex = box.Adress.Row - 1;
                int stillageIndex = box.Adress.Stillage - 1;
                sortedBoxesList[rowIndex][stillageIndex].Add(box);
            }
        }

        foreach (var rowList in sortedBoxesList)
        {
            foreach (var stillageList in rowList)
            {
                stillageList.Sort(new StillagePlacementComparer());
            }
        }

        return sortedBoxesList;
    }

    public List<DepoCell> GetDepo()
    {
        SaveLoader.Save(_boxes);
        var depo = new List<DepoCell>();
        var depoLimit = SaveLoader.GetDepoLimit();

        foreach (var depoCellNumber in depoLimit)
        {
            var depoCell = new DepoCell();
            depoCell.Number = depoCellNumber;
            depo.Add(depoCell);
        }

        foreach (var box in _boxes)
        {
            if(box.Adress.DepoCellNumber != 0)
            {
                foreach (var depoCell in depo)
                {
                    if(depoCell.Number == box.Adress.DepoCellNumber)
                    {
                        if (box.Adress.DepoCellSide == DepoCellSide.Левая)
                            depoCell.LeftSide.Add(box);

                        if (box.Adress.DepoCellSide == DepoCellSide.Центр)
                            depoCell.Center.Add(box);

                        if (box.Adress.DepoCellSide == DepoCellSide.Правая)
                            depoCell.RightSide.Add(box);
                    }
                }
            }
        }

        return depo;
    }
}