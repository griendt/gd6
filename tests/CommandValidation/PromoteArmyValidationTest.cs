using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class PromoteArmyValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).HqSettler = Players.Player1;
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(5);
        Players.Player1.InfluencePoints = 300;
    }

    [Test]
    public void ItAcceptsPromotingIfOwnedAndHasAnArmy()
    {
        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                UnitType = Unit.Cavalry,
                Quantity = 1,
            },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }

    [Test]
    public void ItRejectsPromotingToArmy()
    {
        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                UnitType = Unit.Army,
                Quantity = 1,
            },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands.First().IsRejected);
        Assert.That(commands.First().Rejections.First().Reason, Is.EqualTo(RejectReason.InvalidPromotionUnitType));
    }

    [TestCase(2, Unit.Cavalry, 1, true)]
    [TestCase(3, Unit.Cavalry, 1, false)]
    [TestCase(5, Unit.Cavalry, 2, true)]
    [TestCase(6, Unit.Cavalry, 2, false)]
    [TestCase(29, Unit.Cavalry, 10, true)]
    [TestCase(30, Unit.Cavalry, 10, false)]
    public void ItRejectsIfNotEnoughInfluencePoints(int numInfluencePoints, Unit promotionType, int quantity, bool shouldBeRejected)
    {
        Players.Player1.InfluencePoints = numInfluencePoints;
        T(1).Units.AddArmies(quantity);
        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                UnitType = promotionType,
                Quantity = quantity,
            },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands.First().IsRejected, Is.EqualTo(shouldBeRejected));
        if (shouldBeRejected) {
            Assert.That(commands.First().Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
        }
    }

    [Test]
    public void ItRejectsPromotingIfNotOwned()
    {
        T(1).Owner = Players.Player2;

        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                UnitType = Unit.Cavalry,
                Quantity = 1,
            },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.True);
        Assert.That(commands[0].Rejections.First().Reason, Is.EqualTo(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Test]
    public void ItRejectsPromotingIfNotEnoughArmiesAvailableToPromote()
    {
        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                UnitType = Unit.Cavalry,
                Quantity = 6,
            },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.True);
        Assert.That(commands[0].Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientArmies));
    }

    [Test]
    public void ItRejectsPromotingNegativeAmount()
    {
        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                UnitType = Unit.Cavalry,
                Quantity = -3,
            },
        ];

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

        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1 + distance),
                UnitType = Unit.Cavalry,
                Quantity = 1,
            },
        ];

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
        Players.Player2.InfluencePoints = 300;

        List<PromoteArmy> commands =
        [
            new()
            {
                Issuer = Players.Player2,
                Origin = T(2),
                UnitType = Unit.Cavalry,
                Quantity = 1,
            },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected);
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.PromotingTooFarFromOwnedHq));
    }
}