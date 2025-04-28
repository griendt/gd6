using engine.Engine.Commands;

namespace tests.CommandValidation;

public class PromoteArmyToCavalryValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).HqSettler = Players.Player1;
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(5);
    }

    [Test]
    public void ItAcceptsPromotingIfOwnedAndHasAnArmy()
    {
        List<PromoteArmyToCavalry> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }

    [Test]
    public void ItRejectsPromotingIfNotOwned()
    {
        T(1).Owner = Players.Player2;

        List<PromoteArmyToCavalry> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.True);
        Assert.That(commands[0].Rejections.First().Reason, Is.EqualTo(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Test]
    public void ItRejectsPromotingIfNotEnoughArmiesAvailableToPromote()
    {
        List<PromoteArmyToCavalry> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 6 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.True);
        Assert.That(commands[0].Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientArmies));
    }

    [Test]
    public void ItRejectsPromotingNegativeAmount()
    {
        List<PromoteArmyToCavalry> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = -3 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.True);
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.PromotingNegativeQuantity));
    }

    [TestCase(0, false)]
    [TestCase(1, false)]
    [TestCase(2, true)]
    public void ItChecksOwnedHqDistance(int distance, bool shouldBeRejected)
    {
        T(1 + distance).Owner = Players.Player1;
        T(1 + distance).Units.AddArmies(1);
        
        List<PromoteArmyToCavalry> commands = [new() { Issuer = Players.Player1, Origin = T(1 + distance), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.EqualTo(shouldBeRejected));
        if (shouldBeRejected) {
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.PromotingTooFarFromOwnedHq));
        }
    }

    [Test]
    public void ItRejectsPromotingNextToHqOwnedByOtherPlayer()
    {
        T(2).Owner = Players.Player2;
        T(2).Units.AddArmies(1);
        
        List<PromoteArmyToCavalry> commands = [new() { Issuer = Players.Player2, Origin = T(2), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected);
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.PromotingTooFarFromOwnedHq));
    }
}