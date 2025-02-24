using gdvi.Engine.Commands;
using gdvi.Models;

namespace tests.CommandValidation;

public class MoveArmyValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[1].HqSettler = Players.Player1;
        World.Territories[1].Owner = Players.Player1;
    }

    [TestCase(true, false, null)]
    [TestCase(false, true, RejectReason.PlayerDoesNotOwnOriginTerritory)]
    public void ItChecksIfUserOwnsOriginTerritory(bool doesPlayerOwnTerritory, bool expectedRejected, RejectReason? reason)
    {
        World.Territories[1].Owner = doesPlayerOwnTerritory ? Players.Player1 : null;
        World.Territories[1].Units.AddArmies(5);

        var command = new MoveArmy { Issuer = Players.Player1, Origin = World.Territories[1], Target = World.Territories[2] };
        
        CommandValidator.Validate([command], World);
        
        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }

    [TestCase(1, 2, false, null)]
    [TestCase(2, 3, false, null)]
    [TestCase(2, 4, true, RejectReason.TargetNotAdjacentToOrigin)]
    public void ItChecksIfTargetIsAdjacentToOrigin(int originId, int targetId, bool expectedRejected, RejectReason? reason)
    {
        World.Territories[originId].Owner = Players.Player1;
        World.Territories[originId].Units.AddArmies(5);
        
        var command = new MoveArmy { Issuer = Players.Player1, Origin = World.Territories[originId], Target = World.Territories[targetId] };
        
        CommandValidator.Validate([command], World);
        
        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }
}