using engine.Models;

namespace engine.Engine.Commands;

public class MoveArmy : Command, IHasOrigin, IHasPath
{
    public bool IsProcessed;
    public required Territory Origin { get; set; }
    public required List<Territory> Path { get; set; }

    public override Phase Phase()
    {
        return Engine.Phase.Movement;
    }

    public override void Process(World world)
    {
        throw new Exception("MoveArmy should not be processed directly, but via a MoveResolver");
    }

    public void Fail()
    {
        Origin.Units.Pop();
        IsProcessed = true;
    }

    [Validator]
    public static void ValidateOwnerOwnsOrigin(IEnumerable<MoveArmy> commands, World world)
    {
        commands
            .Where(command => command.Origin.Owner != command.Issuer)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidatePathIsConnected(IEnumerable<MoveArmy> commands, World world)
    {
        commands
            .Where(command => Enumerable
                .Range(0, command.Path.Count - 1)
                .Any(index => !world.TerritoryBorders[command.Path[index].Id].Contains(command.Path[index + 1].Id)))
            .Each(command => command.Reject(RejectReason.PathNotConnected));
    }

    [Validator]
    public static void ValidatePathLength(IEnumerable<MoveArmy> commands, World world)
    {
        commands
            .Where(command => !((int[]) [2, 3]).Contains(command.Path.Count))
            .Each(command => command.Reject(RejectReason.InvalidPathLength));
    }

    [Validator]
    public static void ValidateFirstPartOfPathIsEqualToOrigin(IEnumerable<MoveArmy> commands, World world)
    {
        commands
            .Where(command => command.Origin != command.Path[0])
            .Each(command => command.Reject(RejectReason.InvalidPathStartingPoint));
    }
}