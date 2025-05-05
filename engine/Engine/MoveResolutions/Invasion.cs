using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Invasion : MoveResolver
{
    public override void Resolve(List<MoveUnit> moves, World world)
    {
        var origin = moves.First().Path.First();
        var target = moves.First().Path.Second();

        var isIntelligencePresent =
            origin.Constructs.Contains(Construct.Intelligence)
            || origin.Neighbours().Any(neighbour => neighbour.Owner == moves.First().Issuer && neighbour.Constructs.Contains(Construct.Intelligence));

        if (!isIntelligencePresent) {
            Enumerable.Range(1, 2).Each(_ => moves.FirstOrDefault(move => !move.IsProcessed)?.IncurDamage());
        }

        foreach (var move in moves) {
            if (target.IsNeutral) {
                break;
            }

            while (!move.IsProcessed && !target.IsNeutral) {
                Enumerable.Range(1, move.UnitType.Strength()).Each(_ => target.IncurDamage());
                move.IncurDamage();
            }
        }
        
        target.ResetDamage();
        moves.Each(move => move.ResetDamage());
    }
}