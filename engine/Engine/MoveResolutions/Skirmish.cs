using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Skirmish : MoveResolver, IProcessable
{
    public override void Process(World world)
    {
        var commandsByPlayer = Moves.GroupBy(move => move.Issuer).ToList();

        while (commandsByPlayer.Count(group => group.Any(move => !move.IsProcessed)) > 1) {
            foreach (var moves in commandsByPlayer) {
                moves.First().Fail();
            }
        }
    }
}