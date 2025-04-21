using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Invasion : MoveResolver
{
    public override void Resolve(List<MoveArmy> moves, World world)
    {
        moves.Take(2).Each(move => move.Fail());

        foreach (var move in moves) {
            if (move.IsProcessed) {
                continue;
            }

            var target = move.Path.Second();

            if (target.IsNeutral) {
                return;
            }

            target.Units.Pop();
            move.Fail();

            if (target.Units.IsEmpty) {
                target.Neutralize();
                return;
            }
        }
    }
}