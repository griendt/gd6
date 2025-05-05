using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class MoveUnitValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).HqSettler = Players.Player1;
        T(1).Owner = Players.Player1;
    }

    [TestCase(Unit.Army, true, false, null)]
    [TestCase(Unit.Army, false, true, RejectReason.PlayerDoesNotOwnOriginTerritory)]
    [TestCase(Unit.Cavalry, true, false, null)]
    [TestCase(Unit.Cavalry, false, true, RejectReason.PlayerDoesNotOwnOriginTerritory)]
    public void ItChecksIfUserOwnsOriginTerritory(Unit unitType, bool doesPlayerOwnTerritory, bool expectedRejected, RejectReason? reason)
    {
        T(1).Owner = doesPlayerOwnTerritory ? Players.Player1 : null;
        T(1).Units.Add(unitType, 5);

        var command = new MoveUnit
        {
            Issuer = Players.Player1,
            UnitType = unitType,
            Origin = T(1),
            Path = [T(1), T(2)],
        };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }

    [TestCase(Unit.Army)]
    [TestCase(Unit.Cavalry)]
    public void ItChecksThatOriginIsFirstPartOfPath(Unit unitType)
    {
        T(1).Owner = Players.Player1;
        T(1).Units.Add(unitType, 5);

        var command = new MoveUnit
        {
            Issuer = Players.Player1,
            UnitType = unitType,
            Origin = T(1),
            Path = [T(2), T(3)],
        };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InvalidPathStartingPoint));
    }

    [TestCase(Unit.Army, 1, 2, false, null)]
    [TestCase(Unit.Army, 2, 3, false, null)]
    [TestCase(Unit.Army, 2, 4, true, RejectReason.PathNotConnected)]
    [TestCase(Unit.Cavalry, 1, 2, false, null)]
    [TestCase(Unit.Cavalry, 2, 3, false, null)]
    [TestCase(Unit.Cavalry, 2, 4, true, RejectReason.PathNotConnected)]
    public void ItChecksIfTargetIsAdjacentToOrigin(Unit unitType, int originId, int targetId, bool expectedRejected, RejectReason? reason)
    {
        World.Territories[originId].Owner = Players.Player1;
        World.Territories[originId].Units.Add(unitType, 5);

        var command = new MoveUnit
        {
            Issuer = Players.Player1,
            UnitType = unitType,
            Origin = T(originId),
            Path = [T(originId), T(targetId)],
        };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }

    [TestCase(2, false, null)]
    [TestCase(3, false, null)]
    [TestCase(4, true, RejectReason.InvalidPathLength)]
    public void ItChecksThatArmyCanMoveAtMostTwoTimes(int numMoves, bool expectedRejected, RejectReason? reason)
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(5);

        var command = new MoveUnit
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Path = Enumerable.Range(1, numMoves).Select(i => World.Territories[i]).ToList(),
        };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }

    [TestCase(2, false, null)]
    [TestCase(3, false, null)]
    [TestCase(4, false, null)]
    [TestCase(5, false, null)]
    [TestCase(6, true, RejectReason.InvalidPathLength)]
    public void ItChecksThatCavalryCanMoveAtMostFourTimes(int numMoves, bool expectedRejected, RejectReason? reason)
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddCavalries(5);

        var command = new MoveUnit
        {
            Issuer = Players.Player1,
            UnitType = Unit.Cavalry,
            Origin = T(1),
            Path = Enumerable.Range(1, numMoves).Select(i => World.Territories[i]).ToList(),
        };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.EqualTo(expectedRejected));
        if (reason != null) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(reason));
        }
    }
}