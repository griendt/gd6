using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Distribute : MoveResolver
{
    public override void Resolve(List<MoveArmy> moves, World world)
    {
        foreach (var move in moves) {
            // Note: this assumes that the origin is also part of the path.
            if (move.Path.Count < 2 || move.IsProcessed) {
                move.IsProcessed = true;
                continue;
            }

            move.Path.First().Units.Pop();
            move.Path.RemoveAt(0);

            move.Path.First().Units.AddArmy();
            move.Path.First().Owner = move.Issuer;

            if (move.Path.Count < 2) {
                move.IsProcessed = true;
            }
        }
    }
}