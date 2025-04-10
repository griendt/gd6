using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Skirmish : MoveResolver
{
    public override void Resolve(List<MoveArmy> moves, World world)
    {
        var commandsByPlayer = moves.GroupBy(move => move.Issuer).ToList();

        while (commandsByPlayer.Count(group => group.Any(move => !move.IsProcessed)) > 1) {
            foreach (var movesForPlayer in commandsByPlayer) {
                movesForPlayer.First().Fail();
            }
        }
    }
}