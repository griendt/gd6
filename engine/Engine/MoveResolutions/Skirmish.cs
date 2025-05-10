using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Skirmish : MoveResolver
{
    public override void Resolve(List<MoveUnit> moves, World world)
    {
        var commandsByPlayer = moves.GroupBy(move => move.Issuer).ToList();
        
        // If a player issued commands with only 0 health units,
        // do not cause this to deal damage to other players.
        // Instead, mark those all immediately as failed.
        commandsByPlayer.Where(group => group.Sum(move => move.UnitType.Health()) == 0)
            .Each(group => group.Each(command => command.IncurDamage()));

        while (commandsByPlayer.Count(group => group.Any(move => !move.IsProcessed)) > 1) {
            foreach (var movesForPlayer in commandsByPlayer) {
                // Any moves with 0 health units are all discarded (if at current highest priority)
                movesForPlayer.TakeWhile(move => move.UnitType.Health() == 0)
                    .Each(move => move.IncurDamage());
                
                movesForPlayer.FirstOrDefault(move => move.UnitType.Health() > 0)?.IncurDamage();
            }
        }
        
        moves.Each(move => move.ResetDamage());
    }
}