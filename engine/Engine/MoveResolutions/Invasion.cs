using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public class Invasion : MoveResolver
{
    public override void Resolve(List<MoveArmy> moves, World world)
    {
        var fortressPenaltyPaid = 0;
        
        moves.Take(2).Each(move => move.Fail());

        foreach (var move in moves) {
            if (move.IsProcessed) {
                continue;
            }

            var target = move.Path.Second();

            if (target.IsNeutral) {
                return;
            }

            if (target.Constructs.Contains(Construct.Fortress)) {
                move.Fail();
                fortressPenaltyPaid++;

                if (fortressPenaltyPaid >= 2) {
                    // The second army that gets to the fortress will ruin it.
                    target.Constructs.Remove(Construct.Fortress);
                    target.Constructs.Add(Construct.Ruin);
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