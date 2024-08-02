namespace CityLayoutCalculator;

public static class CityLayouts
{
    public static CityLayout Adamazium { get; } = new(
        citySpaceCount: 11,
        cityHallConnections: new[] { 1, 3, 4, 9 },
        connections: new[]
        {
            (0, 1),
            (1, 2), (1, 3),
            (3, 4), (3, 5),
            (4, 7),
            (5, 6),
            (6, 7),
            (7, 8), (7, 9),
            (8, 9),
            (9, 10),
        }
    );
}

public sealed class CityLayout(int citySpaceCount, IEnumerable<int> cityHallConnections, IEnumerable<(int From, int To)> connections)
{
    public int CitySpaceCount { get; } = citySpaceCount;
    public IReadOnlyList<int> CityHallConnections { get; } = cityHallConnections.ToArray();
    public IReadOnlyList<(int From, int To)> Connections { get; } = connections.ToArray();
}
