namespace SporeCalculator;

public sealed class City
{
    private const int HappinessLowerCap = -5;
    private const int HappinessUpperCap = 5;
    private const int FactoryConnectionIncome = 400;

    public CityHall Hall { get; }
    public CitySpace[] CitySpaces { get; }

    public int MinEmptyCount { get; set; }
    public int MaxEmptyCount { get; set; }

    public int CitySpaceCount => CitySpaces.Length;

    public City(CityHall cityHall, CitySpace[] citySpaces)
    {
        Hall = cityHall;
        CitySpaces = citySpaces;
    }

    [Obsolete("Use CityBuilder to create a City")]
    public City()
    {
        Hall = new();
        var citySpaces = new CitySpace[11];
        for (int i = 0; i < 11; i++)
        {
            var citySpace = new CitySpace(i);
            citySpaces[i] = citySpace;
        }

        CitySpaces = citySpaces;

        // Connections:
        // 0 -> 1
        // 1 -> 0, 2, 3, hall
        // 2 -> 1
        // 3 -> 1, 4, 5, hall
        // 4 -> 3, 7, hall
        // 5 -> 3, 6
        // 6 -> 5, 7
        // 7 -> 4, 8, 9
        // 8 -> 7, 9
        // 9 -> 7, 8, 10, hall
        // 10 -> 9
        Building.ConnectBothWays(citySpaces[0], citySpaces[1]);
        Building.ConnectBothWays(citySpaces[1], citySpaces[2]);
        Building.ConnectBothWays(citySpaces[1], citySpaces[3]);
        Building.ConnectBothWays(citySpaces[1], Hall);
        Building.ConnectBothWays(citySpaces[3], citySpaces[4]);
        Building.ConnectBothWays(citySpaces[3], citySpaces[5]);
        Building.ConnectBothWays(citySpaces[3], Hall);
        Building.ConnectBothWays(citySpaces[4], citySpaces[7]);
        Building.ConnectBothWays(citySpaces[4], Hall);
        Building.ConnectBothWays(citySpaces[5], citySpaces[6]);
        Building.ConnectBothWays(citySpaces[6], citySpaces[7]);
        Building.ConnectBothWays(citySpaces[7], citySpaces[8]);
        Building.ConnectBothWays(citySpaces[7], citySpaces[9]);
        Building.ConnectBothWays(citySpaces[8], citySpaces[9]);
        Building.ConnectBothWays(citySpaces[9], citySpaces[10]);
        Building.ConnectBothWays(citySpaces[9], Hall);
    }

    public bool IsCityLegal()
    {
        int emptyCount = 0;
        // foreach (var citySpace in CitySpaces.AsSpan())
        foreach (var citySpace in CitySpaces)
        {
            if (citySpace.Type is BuildingType.Empty)
            {
                emptyCount++;
                if (emptyCount > MaxEmptyCount)
                {
                    return false;
                }

                continue;
            }

            bool allEmpty = true;
            // foreach (var connection in CollectionsMarshal.AsSpan(citySpace.Connections))
            foreach (var connection in citySpace.Connections)
            {
                allEmpty &= connection.Type is BuildingType.Empty;
            }

            if (allEmpty)
            {
                return false;
            }
        }

        return emptyCount >= MinEmptyCount;
    }

    public CityStats CalculateStats()
    {
        int happiness = 0;
        int incomeConnections = 0;
        // foreach (var citySpace in CitySpaces.AsSpan())
        foreach (var citySpace in CitySpaces)
        {
            if (citySpace.Type is BuildingType.Entertainment)
            {
                happiness++; // Entertainment buildings give 1 happiness
                // foreach (var connection in CollectionsMarshal.AsSpan(citySpace.Connections))
                foreach (var connection in citySpace.Connections)
                {
                    switch (connection.Type)
                    {
                        case BuildingType.House or BuildingType.CityHall:
                            happiness++;
                            break;
                        case BuildingType.Factory:
                            happiness--;
                            break;
                    }
                }
            }
            else if (citySpace.Type is BuildingType.Factory)
            {
                happiness--;
                // foreach (var connection in CollectionsMarshal.AsSpan(citySpace.Connections))
                foreach (var connection in citySpace.Connections)
                {
                    if (connection.Type is BuildingType.House or BuildingType.CityHall)
                    {
                        incomeConnections++;
                    }
                }
            }
        }

        return new()
        {
            Happiness = Math.Clamp(happiness, HappinessLowerCap, HappinessUpperCap),
            Income = incomeConnections * FactoryConnectionIncome
        };
    }

    public BuildingType[] GetSnapshot()
    {
        return CitySpaces.Select(cs => cs.Type).ToArray();
    }

    public void SetSnapshot(BuildingType[] snapshot)
    {
        if (snapshot.Length != CitySpaces.Length)
        {
            throw new ArgumentException("Snapshot length does not match city length", nameof(snapshot));
        }

        for (int i = 0; i < snapshot.Length; i++)
        {
            CitySpaces[i].Type = snapshot[i];
        }
    }

    public void PrintSummary()
    {
        Console.WriteLine("City Summary:");
        var stats = CalculateStats();
        Console.WriteLine($"Happiness: {stats.Happiness}");
        Console.WriteLine($"Income: {stats.Income}");
        // Count each buildingType
        var buildingTypeCounts = new BuildingCounts();
        // foreach (var citySpace in CitySpaces.AsSpan())
        foreach (var citySpace in CitySpaces)
        {
            Console.WriteLine($"{citySpace.Id}: {citySpace.Type}");
            buildingTypeCounts.Increment(citySpace.Type);
        }

        Console.WriteLine($"Empty: {buildingTypeCounts.Empty}");
        Console.WriteLine($"House: {buildingTypeCounts.House}");
        Console.WriteLine($"Entertainment: {buildingTypeCounts.Entertainment}");
        Console.WriteLine($"Factory: {buildingTypeCounts.Factory}");
    }
}

public readonly record struct CityStats(int Happiness, int Income)
{
    public int Score => Happiness < 1 ? 0 : Income + Happiness;
}