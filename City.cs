using System.Runtime.InteropServices;

namespace SporeCalculator;

public sealed class City
{
    private readonly int _minEmptyCount;
    private readonly int _maxEmptyCount;
    private const int HappinessLowerCap = -5;
    private const int HappinessUpperCap = 5;
    private const int WorkConnectionIncome = 400;

    public TownHall Hall { get; }
    public CitySpace[] CitySpaces { get; }

    public int CitySpaceCount => CitySpaces.Length;

    public City(int minEmptyCount = 0, int maxEmptyCount = 0)
    {
        _minEmptyCount = minEmptyCount;
        _maxEmptyCount = maxEmptyCount;
        Hall = new TownHall();
        var citySpaces = new CitySpace[11];
        for(int i = 0; i < 11; i++)
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
        Building.Connect(citySpaces[0], citySpaces[1]);
        Building.Connect(citySpaces[1], citySpaces[2]);
        Building.Connect(citySpaces[1], citySpaces[3]);
        Building.Connect(citySpaces[1], Hall);
        Building.Connect(citySpaces[3], citySpaces[4]);
        Building.Connect(citySpaces[3], citySpaces[5]);
        Building.Connect(citySpaces[3], Hall);
        Building.Connect(citySpaces[4], citySpaces[7]);
        Building.Connect(citySpaces[4], Hall);
        Building.Connect(citySpaces[5], citySpaces[6]);
        Building.Connect(citySpaces[6], citySpaces[7]);
        Building.Connect(citySpaces[7], citySpaces[8]);
        Building.Connect(citySpaces[7], citySpaces[9]);
        Building.Connect(citySpaces[8], citySpaces[9]);
        Building.Connect(citySpaces[9], citySpaces[10]);
        Building.Connect(citySpaces[9], Hall);
    }

    public bool IsCityLegal()
    {
        int emptyCount = 0;
        foreach (var citySpace in CitySpaces.AsSpan())
        {
            if (citySpace.Type is BuildingType.Empty)
            {
                emptyCount++;
                if (emptyCount > _maxEmptyCount)
                {
                    return false;
                }
                continue;
            }

            bool allEmpty = true;
            foreach (var connection in CollectionsMarshal.AsSpan(citySpace.Connections))
            {
                allEmpty &= connection.Type is BuildingType.Empty;
            }

            if (allEmpty)
            {
                return false;
            }
        }

        return emptyCount >= _minEmptyCount;
    }

    public CityStats CalculateStats()
    {
        int happiness = 0;
        int incomeConnections = 0;
        foreach (var citySpace in CitySpaces.AsSpan())
        {
            if (citySpace.Type is BuildingType.Entertainment)
            {
                happiness++; // Entertainment buildings give 1 happiness
                foreach (var connection in CollectionsMarshal.AsSpan(citySpace.Connections))
                {
                    switch (connection.Type)
                    {
                        case BuildingType.House or BuildingType.TownHall:
                            happiness++;
                            break;
                        case BuildingType.Work:
                            happiness--;
                            break;
                    }
                }
            }
            else if(citySpace.Type is BuildingType.Work)
            {
                happiness--;
                foreach (var connection in CollectionsMarshal.AsSpan(citySpace.Connections))
                {
                    if (connection.Type is BuildingType.House or BuildingType.TownHall)
                    {
                        incomeConnections++;
                    }
                }
            }
        }

        return new()
        {
            Happiness = Math.Clamp(happiness, HappinessLowerCap, HappinessUpperCap),
            Income = incomeConnections * WorkConnectionIncome
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
        foreach (var citySpace in CitySpaces.AsSpan())
        {
            Console.WriteLine($"{citySpace.Id}: {citySpace.Type}");
            buildingTypeCounts.Increment(citySpace.Type);
        }

        Console.WriteLine($"Empty: {buildingTypeCounts.Empty}");
        Console.WriteLine($"House: {buildingTypeCounts.House}");
        Console.WriteLine($"Entertainment: {buildingTypeCounts.Entertainment}");
        Console.WriteLine($"Work: {buildingTypeCounts.Work}");
    }
}

public readonly record struct CityStats(int Happiness, int Income)
{
    public int Score => Happiness < 1 ? 0 : Income + Happiness;
}