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

    public static List<MoveArmy> ProcessSkirmish(List<MoveArmy> commands)
    {
        var commandsByPlayer = new Dictionary<Player, List<MoveArmy>>();

        foreach (var command in commands) {
            if (!commandsByPlayer.ContainsKey(command.Issuer)) {
                commandsByPlayer[command.Issuer] = [];
            }

            commandsByPlayer[command.Issuer].Add(command);
        }

        while (true) {
            if (commandsByPlayer.Count(group => group.Value.Count > 0) <= 1) {
                // If only one (or zero) players have moves left, break out of the loop.
                break;
            }

            foreach (var (player, moves) in commandsByPlayer) {
                // TODO: keep into account command priority (i.e. pop lowest priority first!)
                moves.First().Fail();
                moves.RemoveAt(0);
            }
        }

        foreach (var (player, remainingCommands) in commandsByPlayer) {
            return remainingCommands;
        }

        return [];
    }
}