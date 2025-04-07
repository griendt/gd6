using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class UseDynamiteValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).HqSettler = Players.Player1;
        T(1).Owner = Players.Player1;
    }

    private static void SetUpDynamites(Player player, int quantity = 1)
    {
        foreach (var i in Enumerable.Range(1, quantity)) {
            player.Inventory.Add(Item.Dynamite);
        }
    }

    [TestCase(0, true, RejectReason.InsufficientAmountOfItems)]
    [TestCase(1, false, null)]
    [TestCase(47, false, null)]
    public void ItChecksIfDynamiteIsInInventory(int numDynamitesInInventory, bool expectedRejected, RejectReason? reason)
    {
        SetUpDynamites(Players.Player1, numDynamitesInInventory);
        var command = new UseDynamite { Issuer = Players.Player1, Origin = T(1), Target = T(2) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }

    [TestCase(true, false, null)]
    [TestCase(false, true, RejectReason.PlayerDoesNotOwnOriginTerritory)]
    public void ItChecksIfUserOwnsOriginTerritory(bool doesPlayerOwnTerritory, bool expectedRejected, RejectReason? reason)
    {
        SetUpDynamites(Players.Player1);
        T(1).Owner = doesPlayerOwnTerritory ? Players.Player1 : null;
        var command = new UseDynamite { Issuer = Players.Player1, Origin = T(1), Target = T(2) };

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
        SetUpDynamites(Players.Player1);
        World.Territories[originId].Owner = Players.Player1;
        var command = new UseDynamite { Issuer = Players.Player1, Origin = World.Territories[originId], Target = World.Territories[targetId] };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }
}