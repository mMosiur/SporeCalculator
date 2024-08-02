namespace SporeCalculator;

public abstract class Building(BuildingType type)
{
    public List<Building> Connections { get; } = new(4);
    public BuildingType Type { get; set; } = type;

    public static void Connect(Building a, Building b)
    {
        a.Connections.Add(b);
        b.Connections.Add(a);
    }

    public static char TypeToChar(BuildingType type)
    {
        return type switch
        {
            BuildingType.Empty => '_',
            BuildingType.House => 'H',
            BuildingType.Entertainment => 'E',
            BuildingType.Work => 'W',
            BuildingType.TownHall => 'T',
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid building type")
        };
    }
}

public sealed class TownHall() : Building(BuildingType.TownHall)
{
}

public sealed class CitySpace : Building
{
    public int Id { get; }

    public CitySpace(int id, BuildingType type = default) : base(type)
    {
        Id = id;
    }
}

public enum BuildingType
{
    Empty = 0,

    House = 1,
    Entertainment = 2,
    Work = 3,
    TownHall = 4,
}
