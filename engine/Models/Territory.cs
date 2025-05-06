using System.ComponentModel.DataAnnotations;
using System.Diagnostics.Contracts;

namespace engine.Models;

public class Territory(World world)
{
    public readonly HashSet<Construct> Constructs = [];
    public int Mines = 0;

    // Purely for web
    public readonly List<(int, int)> Coordinates = [];
    public readonly UnitCollection Units = new();

    private int _incurredDamage;
    private Player? _owner;
    public Player? HqSettler = null;

    [Key]
    public required int Id;

    public bool IsWasteland = false;

    /// <summary>
    ///     Whenever a territory changes owner, set the occupation turn number.
    ///     We can use this to decide which territories a player has owned the
    ///     longest consecutive amount of time. This is needed for when players
    ///     no longer own a HQ and still want to spawn units.
    /// </summary>
    public int Loyalty;

    public Player? Owner
    {
        get => _owner;
        set
        {
            if (_owner != value) {
                Loyalty = 0;
            }
            _owner = value;
        }
    }

    public bool ContainsHq => HqSettler != null;
    public bool IsNeutral => Owner == null;

    [Pure]
    public IEnumerable<Territory> Neighbours() => world.TerritoryBorders
        .GetValueOrDefault(Id, [])
        .Select(id => world.Territories[id]);

    [Pure]
    public bool IsNeighbour(Territory other) => Neighbours().Contains(other);

    public void Neutralize()
    {
        Owner = null;
        Constructs.Remove(Construct.Bivouac);
        _incurredDamage = 0;
    }

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
        Constructs.Remove(Construct.Watchtower);
        Constructs.Remove(Construct.Library);
    }

    public void ResetDamage() => _incurredDamage = 0;

    public void IncurDamage()
    {
        _incurredDamage++;

        if (Constructs.Contains(Construct.Watchtower)) {
            if (_incurredDamage >= 2) {
                Constructs.Remove(Construct.Watchtower);
                _incurredDamage -= 2;
            }

            return;
        }

        foreach (var unitType in UnitExtensions.DefenseOrder()) {
            if (Units.OfType(unitType) > 0 && unitType.Health() <= _incurredDamage) {
                Units.Pop(unitType);
                _incurredDamage = 0;
                break;
            }
        }

        if (Units.IsEmpty) {
            Neutralize();
        }
    }

    public void PromoteArmiesToCavalry(int quantity)
    {

    }
}