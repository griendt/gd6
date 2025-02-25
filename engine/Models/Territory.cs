namespace gdvi.Models;

public class Territory(World world)
{
    public required int Id;

    public Player? Owner
    {
        get => _owner;
        set
        {
            _owner = value;
            Loyalty = 0;
        }
    }

    public Player? HqSettler = null;
    public bool IsWasteland = false;
    public readonly UnitCollection Units = new();
    public bool ContainsHq => HqSettler != null;
    public bool IsNeutral => Owner == null;
    public readonly HashSet<Construct> Constructs = [];

    private Player? _owner;

    /// <summary>
    /// Whenever a territory changes owner, set the occupation turn number.
    /// We can use this to decide which territories a player has owned the
    /// longest consecutive amount of time. This is needed for when players
    /// no longer own a HQ and still want to spawn units.
    /// </summary>
    public int Loyalty = 0;

    public IEnumerable<Territory> Neighbours() => world.TerritoryBorders
        .GetValueOrDefault(Id, [])
        .Select(id => world.Territories[id]);

    public bool IsNeighbour(Territory other) => Neighbours().Contains(other);

    private void Neutralize() => Owner = null;

    public void Build(Construct construct) => Constructs.Add(construct);

    public void ApplyWastelandPenalty()
    {
        if (!IsWasteland) {
            return;
        }

        Units.TryPop();

        if (Units.IsEmpty) {
            Neutralize();
        }
    }

    public void ApplyDynamitePenalty()
    {
        Units.TryPop();

        if (Units.IsEmpty) {
            Neutralize();
        }
        
        Constructs.Remove(Construct.Bivouac);
        if (Constructs.Remove(Construct.Fortress)) {
            Constructs.Add(Construct.Ruin);
        }
    }
}