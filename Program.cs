using System.Diagnostics;
using SporeCalculator;

// var city = new City();

// Calculate optimal snapshot

void CalculateAndPrintBestSnapshots(City city)
{
    var totalArrangements = Math.Pow(4, city.CitySpaceCount);
    var legalArrangements = 0;
    var bestStats = new CityStats { Happiness = -6, Income = -1 };
    var bestStatSnapshots = new List<BuildingType[]>();
    for (var i = 0; i < totalArrangements; i++)
    {
        int temp = i;
        for (int j = 0; j < 11; j++)
        {
            city.CitySpaces[j].Type = (temp % 4) switch
            {
                0 => BuildingType.Empty,
                1 => BuildingType.House,
                2 => BuildingType.Entertainment,
                3 => BuildingType.Work,
                _ => throw new UnreachableException()
            };
            temp >>= 2;
        }

        if (city.IsCityLegal())
        {
            legalArrangements++;
            var stats = city.CalculateStats();
            if (stats.Score >= bestStats.Score)
            {
                if (stats.Score == bestStats.Score)
                {
                    bestStatSnapshots.Add(city.GetSnapshot());
                }
                else
                {
                    bestStats = stats;
                    bestStatSnapshots.Clear();
                    bestStatSnapshots.Add(city.GetSnapshot());
                }
            }
        }
    }

    Console.WriteLine("{0} ({1}), [{2} appeared {3}]", legalArrangements, (double)legalArrangements / totalArrangements, bestStats, bestStatSnapshots.Count);

    foreach (var snapshot in bestStatSnapshots)
    {
        string snapshotString = string.Join(", ", snapshot.Select((s, i) => $"{i}:{Building.TypeToChar(s)}"));
        var buildingCounts = new BuildingCounts(snapshot);
        string countsString = string.Join(", ", buildingCounts.Select(kvp => $"{Building.TypeToChar(kvp.Key)}:{kvp.Value}"));
        Console.WriteLine($"-- {snapshotString} ({countsString})");
    }
}

// Get path to optimal snapshot

// var snapshot = new[]
// {
//     BuildingType.House,
//     BuildingType.Work,
//     BuildingType.House,
//     BuildingType.House,
//     BuildingType.Work,
//     BuildingType.Entertainment,
//     BuildingType.Entertainment,
//     BuildingType.House,
//     BuildingType.House,
//     BuildingType.Work,
//     BuildingType.House
// };
//
// city.SetSnapshot(snapshot);
// city.PrintSummary();

for (int i = 0; i < 10; i++)
{
    var city = new City(i, i);
    Console.WriteLine($"City with {i} missing buildings:");
    CalculateAndPrintBestSnapshots(city);
    Console.WriteLine();
}
