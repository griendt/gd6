using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Skirmish : IProcessable
{
    public required List<MoveArmy> Moves;

    public void Process(World world)
    {
        var commandsByPlayer = new Dictionary<Player, List<MoveArmy>>();

        foreach (var command in Moves) {
            if (!commandsByPlayer.ContainsKey(command.Issuer)) {
                commandsByPlayer[command.Issuer] = [];
            }

            commandsByPlayer[command.Issuer].Add(command);
        }

        while (true) {
            if (commandsByPlayer
                    .Count(group => group.Value
                        .Any(move => !move.IsProcessed))
                <= 1) {
                // If only one (or zero) players have moves left, break out of the loop.
                break;
            }

            foreach (var (player, moves) in commandsByPlayer) {
                // TODO: keep into account command priority (i.e. pop lowest priority first!)
                moves.First().Fail();
            }
        }
    }
}