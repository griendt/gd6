using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Invasion : MoveResolver
{
    public override void Resolve(List<MoveArmy> moves, World world)
    {
        var watchTowerPenaltyPaid = 0;

        moves.Take(2).Each(move => move.Fail());

        foreach (var move in moves) {
            if (move.IsProcessed) {
                continue;
            }

            var target = move.Path.Second();

            if (target.IsNeutral) {
                return;
            }

            if (target.Constructs.Contains(Construct.Watchtower)) {
                move.Fail();
                watchTowerPenaltyPaid++;

                if (watchTowerPenaltyPaid >= 2) {
                    // The second army that gets to the watchtower will ruin it.
                    target.Constructs.Remove(Construct.Watchtower);
                }

                continue;
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