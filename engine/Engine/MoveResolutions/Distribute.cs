using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Distribute : MoveResolver
{
    public override void Resolve(List<MoveUnit> moves, World world)
    {
        foreach (var move in moves) {
            // Note: this assumes that the origin is also part of the path.
            if (move.Path.Count < 2 || move.IsProcessed) {
                move.IsProcessed = true;
                continue;
            }

            var target = move.Path.Second();

            while (target.Mines > 0 && !move.IsProcessed && target.Owner != move.Issuer) {
                move.IncurDamage();
                target.Mines--;
            }

            if (move.IsProcessed) {
                continue;
            }

            move.Path.First().Units.Pop(move.UnitType);
            target.Units.Add(move.UnitType);
            target.Owner = move.Issuer;
            move.Path.RemoveAt(0);

            if (move.Path.Count < 2) {
                move.IsProcessed = true;
            }
        }
    }
}