using System.ComponentModel.DataAnnotations;
using engine.Models;

namespace engine.Engine.Commands;

public class MoveUnit : Command, IHasOrigin, IHasPath
{
    public bool IsProcessed;
    public required Territory Origin { get; set; }
    public required List<Territory> Path { get; set; }
    public virtual Unit UnitType() => Unit.Army;
    public virtual int MaxMovements() => 2;

    private int _incurredDamage = 0;
    
    public override Phase Phase()
    {
        return Engine.Phase.Movement;
    }

    public override void Process(World world)
    {
        throw new Exception("MoveArmy should not be processed directly, but via a MoveResolver");
    }

    public void IncurDamage(int damage = 1)
    {
        _incurredDamage += damage;
        if (_incurredDamage >= UnitType().Health()) {
            Fail();
        }
    }

    public void Fail()
    {
        Origin.Units.Pop(UnitType());
        IsProcessed = true;
    }

    [Validator]
    public static void ValidateOwnerOwnsOrigin(IEnumerable<MoveUnit> commands, World world)
    {
        commands
            .Where(command => command.Origin.Owner != command.Issuer)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidatePathIsConnected(IEnumerable<MoveUnit> commands, World world)
    {
        commands
            .Where(command => Enumerable
                .Range(0, command.Path.Count - 1)
                .Any(index => !world.TerritoryBorders[command.Path[index].Id].Contains(command.Path[index + 1].Id)))
            .Each(command => command.Reject(RejectReason.PathNotConnected));
    }

    [Validator]
    public static void ValidatePathLength(IEnumerable<MoveUnit> commands, World world)
    {
        commands
            .Where(command => command.Path.Count < 2 || command.Path.Count > command.MaxMovements() + 1)
            .Each(command => command.Reject(RejectReason.InvalidPathLength));
    }

    [Validator]
    public static void ValidateFirstPartOfPathIsEqualToOrigin(IEnumerable<MoveUnit> commands, World world)
    {
        commands
            .Where(command => command.Origin != command.Path[0])
            .Each(command => command.Reject(RejectReason.InvalidPathStartingPoint));
    }
}