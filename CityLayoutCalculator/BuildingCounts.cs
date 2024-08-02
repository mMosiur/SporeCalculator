using System.Collections;

namespace CityLayoutCalculator;

public readonly struct BuildingCounts : IReadOnlyDictionary<BuildingType, int>
{
    private static readonly BuildingType[] StaticKeys = [BuildingType.Empty, BuildingType.House, BuildingType.Entertainment, BuildingType.Factory];

    private readonly int[] _counts;

    public BuildingCounts()
    {
        _counts = [0, 0, 0, 0];
    }

    public BuildingCounts(IEnumerable<BuildingType> snapshot)
    {
        Span<int> counts = stackalloc int[4];
        foreach (var type in snapshot)
        {
            ThrowIfKeyInvalid(type);
            counts[(int)type]++;
        }

        _counts = counts.ToArray();
    }

    public int Empty => _counts[(int)BuildingType.Empty];
    public int House => _counts[(int)BuildingType.House];
    public int Entertainment => _counts[(int)BuildingType.Entertainment];
    public int Factory => _counts[(int)BuildingType.Factory];

    public int Count => StaticKeys.Length;

    private static bool IsKeyValid(BuildingType key)
    {
        int index = (int)key;
        return index >= 0 && index < StaticKeys.Length;
    }

    public bool ContainsKey(BuildingType key)
    {
        return IsKeyValid(key);
    }

    public bool TryGetValue(BuildingType key, out int value)
    {
        if (IsKeyValid(key))
        {
            value = _counts[(int)key];
            return true;
        }

        value = default;
        return false;
    }

    private static void ThrowIfKeyInvalid(BuildingType key)
    {
        if (!IsKeyValid(key))
        {
            throw new KeyNotFoundException($"Building type '{key}' not found");
        }
    }

    public int this[BuildingType type]
    {
        get
        {
            ThrowIfKeyInvalid(type);
            return _counts[(int)type];
        }
    }

    public IEnumerable<BuildingType> Keys => StaticKeys;

    public IEnumerable<int> Values => _counts;

    public void AddTo(BuildingType type, int count)
    {
        ThrowIfKeyInvalid(type);
        _counts[(int)type] += count;
    }

    public void Increment(BuildingType type)
    {
        ThrowIfKeyInvalid(type);
        _counts[(int)type]++;
    }

    public IEnumerator<KeyValuePair<BuildingType, int>> GetEnumerator()
    {
        for (int i = 0; i < StaticKeys.Length; i++)
        {
            yield return new(StaticKeys[i], _counts[i]);
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
