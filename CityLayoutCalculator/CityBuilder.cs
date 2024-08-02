namespace CityLayoutCalculator;

public sealed class CityBuilder
{
    private readonly int _spaceCount;
    private readonly IReadOnlyList<HashSet<int>> _connections;
    private readonly HashSet<int> _hallConnections;
    private int _minEmptyCount;
    private int _maxEmptyCount;

    public CityBuilder(int spaceCount)
    {
        _spaceCount = spaceCount;
        _hallConnections = new();
        var connections = new HashSet<int>[spaceCount];
        for (int i = 0; i < spaceCount; i++)
        {
            connections[i] = new(1);
        }

        _connections = connections;
    }

    public CityBuilder(CityLayout layout) : this(layout.CitySpaceCount, layout.CityHallConnections, layout.Connections)
    {
    }

    public CityBuilder(int spaceCount, IEnumerable<int> hallConnections, IEnumerable<(int From, int To)> connections) : this(spaceCount)
    {
        AddHallConnections(hallConnections);
        AddConnections(connections);
    }

    public CityBuilder SetEmptyCount(int minEmptyCount, int maxEmptyCount)
    {
        _minEmptyCount = minEmptyCount;
        _maxEmptyCount = maxEmptyCount;
        return this;
    }

    public CityBuilder AddHallConnection(int to)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(to);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(to, _spaceCount);

        _hallConnections.Add(to);
        return this;
    }

    public CityBuilder AddHallConnections(IEnumerable<int> connectionsTo)
    {
        foreach (int to in connectionsTo)
        {
            AddHallConnection(to);
        }

        return this;
    }

    public CityBuilder AddConnection(int from, int to)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(from);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(from, _spaceCount);
        ArgumentOutOfRangeException.ThrowIfNegative(to);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(to, _spaceCount);

        _connections[from].Add(to);
        _connections[to].Add(from);
        return this;
    }

    public CityBuilder AddConnections(IEnumerable<(int From, int To)> connections)
    {
        foreach ((int from, int to) in connections)
        {
            AddConnection(from, to);
        }

        return this;
    }

    public City Build()
    {
        var cityHall = new CityHall();
        var citySpaces = new CitySpace[_spaceCount];
        for (int i = 0; i < _spaceCount; i++)
        {
            var citySpace = new CitySpace(i);
            citySpaces[i] = citySpace;
        }

        for (int i = 0; i < _spaceCount; i++)
        {
            foreach (int connection in _connections[i])
            {
                citySpaces[i].Connections.Add(citySpaces[connection]);
            }
        }

        foreach (int hallConnection in _hallConnections)
        {
            var citySpace = citySpaces[hallConnection];
            citySpace.Connections.Add(cityHall);
            cityHall.Connections.Add(citySpace);
        }

        var city = new City(cityHall, citySpaces)
        {
            MinEmptyCount = _minEmptyCount,
            MaxEmptyCount = _maxEmptyCount
        };
        return city;
    }
}