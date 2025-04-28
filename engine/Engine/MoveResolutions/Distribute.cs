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

            move.Path.First().Units.Pop(move.UnitType());
            move.Path.Second().Units.Add(move.UnitType());
            move.Path.Second().Owner = move.Issuer;
            move.Path.RemoveAt(0);

            if (move.Path.Count < 2) {
                move.IsProcessed = true;
            }
        }
    }
}