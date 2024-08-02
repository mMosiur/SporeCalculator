namespace CityLayoutCalculator;

public abstract class Building(BuildingType type)
{
    public List<Building> Connections { get; } = new(4);
    public BuildingType Type { get; set; } = type;

    public static void ConnectBothWays(Building a, Building b)
    {
        a.Connections.Add(b);
        b.Connections.Add(a);
    }
}

public sealed class CityHall() : Building(BuildingType.CityHall)
{
}

public sealed class CitySpace(int id, BuildingType type = BuildingType.Empty) : Building(type)
{
    public int Id { get; } = id;
}

public enum BuildingType
{
    Empty = 0,
    House = 1,
    Entertainment = 2,
    Factory = 3,
    CityHall = 4,
}

public static class BuildingHelper
{
    private const string BuildingChars = "_HEFC";

    public static char TypeToChar(BuildingType type)
    {
        int typeInt = (int)type;
        if (typeInt < 0 || typeInt >= BuildingChars.Length)
        {
            throw new ArgumentException($"Invalid Building type '{type}'", nameof(type));
        }

        return BuildingChars[typeInt];
    }

    public static BuildingType CharToType(char c)
    {
        int charIndex = BuildingChars.IndexOf(c);
        if (charIndex == -1)
        {
            throw new ArgumentException($"Invalid Building character '{c}'", nameof(c));
        }

        return (BuildingType)charIndex;
    }
}
