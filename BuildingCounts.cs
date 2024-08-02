using System.Collections;

namespace SporeCalculator;

public struct BuildingCounts : IReadOnlyDictionary<BuildingType, int>
{
    private static readonly BuildingType[] StaticKeys = new[] { BuildingType.Empty, BuildingType.House, BuildingType.Entertainment, BuildingType.Work };

    public BuildingCounts()
    {
    }

    public BuildingCounts(IEnumerable<BuildingType> snapshot)
    {
        foreach (var type in snapshot)
        {
            Increment(type);
        }
    }

    public int Empty { get; set; }
    public int House { get; set; }
    public int Entertainment { get; set; }
    public int Work { get; set; }


    public int Count { get; } = 4;
    public int Total => Empty + House + Entertainment + Work;

    public bool ContainsKey(BuildingType key)
    {
        return key is BuildingType.Empty or BuildingType.House or BuildingType.Entertainment or BuildingType.Work;
    }

    public bool TryGetValue(BuildingType key, out int value)
    {
        int? val = key switch
        {
            BuildingType.Empty => Empty,
            BuildingType.House => House,
            BuildingType.Entertainment => Entertainment,
            BuildingType.Work => Work,
            _ => null
        };
        if (val is null)
        {
            value = default;
            return false;
        }

        value = val.Value;
        return true;
    }

    public int this[BuildingType type]
    {
        get
        {
            return type switch
            {
                BuildingType.Empty => Empty,
                BuildingType.House => House,
                BuildingType.Entertainment => Entertainment,
                BuildingType.Work => Work,
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid building type")
            };
        }
        set
        {
            switch (type)
            {
                case BuildingType.Empty:
                    Empty = value;
                    break;
                case BuildingType.House:
                    House = value;
                    break;
                case BuildingType.Entertainment:
                    Entertainment = value;
                    break;
                case BuildingType.Work:
                    Work = value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid building type");
            }
        }
    }

    public IEnumerable<BuildingType> Keys => StaticKeys;

    public IEnumerable<int> Values => new[] { Empty, House, Entertainment, Work };

    public void AddTo(BuildingType type, int count)
    {
        switch (type)
        {
            case BuildingType.Empty:
                Empty += count;
                break;
            case BuildingType.House:
                House += count;
                break;
            case BuildingType.Entertainment:
                Entertainment += count;
                break;
            case BuildingType.Work:
                Work += count;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, "Invalid building type");
        }
    }

    public void Increment(BuildingType type)
    {
        AddTo(type, 1);
    }

    public IEnumerator<KeyValuePair<BuildingType, int>> GetEnumerator()
    {
        yield return new(BuildingType.Empty, Empty);
        yield return new(BuildingType.House, House);
        yield return new(BuildingType.Entertainment, Entertainment);
        yield return new(BuildingType.Work, Work);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}