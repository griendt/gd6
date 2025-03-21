using engine.Models;

namespace engine.Engine.Commands;

public class MoveArmy : Command, IHasOrigin, IHasPath
{
    public required Territory Origin { get; set; }
    public required List<Territory> Path { get; set; }
    public override Phase Phase() => Engine.Phase.Movement;

    public override void Process(World world)
    {
        // TODO: Implement. Note: we need to also keep into account conflicting moves
        //  (skirmishes, invasions...)
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
            .Where(command =>
                !world.TerritoryBorders[command.Origin.Id].Contains(command.Path.First().Id) ||
                Enumerable
                    .Range(0, command.Path.Count - 1)
                    .Any(index => !world.TerritoryBorders[command.Path[index].Id].Contains(command.Path[index + 1].Id)))
            .Each(command => command.Reject(RejectReason.PathNotConnected));
    }

    [Validator]
    public static void ValidatePathLength(IEnumerable<MoveArmy> commands, World world)
    {
        commands
            .Where(command => command.Path.Count > 2)
            .Each(command => command.Reject(RejectReason.PathTooLong));
    }
}