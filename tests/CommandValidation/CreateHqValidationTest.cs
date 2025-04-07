using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class CreateHqValidationTest : BaseTest
{
    [Test]
    public void CreatingASingleHqIsValid()
    {
        List<CreateHq> commands = [new() { Issuer = Players.Player1, Origin = T(1) }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }

    [TestCase(2)]
    [TestCase(3)]
    [TestCase(4)]
    [TestCase(5)]
    public void ItRejectsMultiplePlayersBuildingHqsInSameTerritory(int numPlayers)
    {
        List<Player> players = [Players.Player1, Players.Player2, Players.Player3, Players.Player4, Players.Player5];

        var commands = Enumerable.Range(0, numPlayers)
            .Select(i => new CreateHq { Issuer = players[i], Origin = T(1) })
            .ToList();

        CommandValidator.Validate(commands, World);

        foreach (var command in commands) {
            Assert.Multiple(() =>
            {
                Assert.That(command.IsRejected, Is.True);
                Assert.That(command.Rejections, Has.Count.EqualTo(1));
            });

            Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
        }
    }

    [Test]
    public void ItRejectsTwoPlayersBuildingHqsInAdjacentTerritories()
    {
        List<CreateHq> commands =
        [
            new() { Issuer = Players.Player1, Origin = T(1) },
            new() { Issuer = Players.Player2, Origin = T(2) },
        ];

        CommandValidator.Validate(commands, World);
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
            Assert.That(commands[1].Rejections, Has.Count.EqualTo(1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
            Assert.That(commands[1].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
        });
    }

    [Test]
    public void ItRejectsBuildingHqOnTopOfExistingHq()
    {
        T(1).HqSettler = Players.Player1;

        List<CreateHq> commands =
        [
            new() { Issuer = Players.Player2, Origin = T(1) },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToExistingHq));
    }

    [Test]
    public void ItRejectsBuildingHqNextToExistingHq()
    {
        T(1).HqSettler = Players.Player1;

        List<CreateHq> commands =
        [
            new() { Issuer = Players.Player2, Origin = T(2) },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToExistingHq));
    }

    [Test]
    public void ItAllowsBuildingHqNextToExistingHqIfForced()
    {
        T(1).HqSettler = Players.Player1;

        List<CreateHq> commands =
        [
            new() { Issuer = Players.Player2, Origin = T(2), Force = true },
        ];

        CommandValidator.Validate(commands, World);

        Assert.Multiple(() =>
        {
            Assert.That(commands[0].IsRejected, Is.False);
            Assert.That(commands[0].Rejections, Is.Empty);
        });
    }

    [Test]
    public void ItRejectsBuildingHqWithBorderAdjacentToExistingHq()
    {
        T(1).HqSettler = Players.Player1;

        List<CreateHq> commands =
        [
            new() { Issuer = Players.Player2, Origin = T(3) },
        ];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToExistingHq));
    }

    [Test]
    public void ItAllowsBuildingHqAtDistance3OfExistingHq()
    {
        T(1).HqSettler = Players.Player1;

        var command = new CreateHq { Issuer = Players.Player2, Origin = T(4) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.False);
    }

    [Test]
    public void ItRejectsBuildingHqOnOccupiedTerritory()
    {
        T(1).Owner = Players.Player1;

        var command = new CreateHq { Issuer = Players.Player2, Origin = T(1) };

        CommandValidator.Validate([command], World);

        Assert.That(command.Rejections, Has.Count.EqualTo(1));
        Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqOnOccupiedTerritory));
    }

    [TestCase(2)]
    [TestCase(3)]
    [TestCase(47)]
    public void ItRejectsBuildingMultipleHqs(int numHqs)
    {
        var commands = Enumerable
            .Range(1, numHqs)
            .Select(i => new CreateHq { Issuer = Players.Player1, Origin = T(1) })
            .ToList();

        CommandValidator.Validate(commands, World);

        foreach (var command in commands) {
            Assert.That(command.Rejections, Has.Count.EqualTo(1));
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingMultipleHqs));
        }
    }

    [Test]
    public void ItRejectsBuildingHqIfPlayerAlreadyHasHq()
    {
        T(1).HqSettler = Players.Player1;

        var command = new CreateHq { Issuer = Players.Player1, Origin = T(4) };

        CommandValidator.Validate([command], World);

        Assert.That(command.Rejections, Has.Count.EqualTo(1));
        Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqWhenPlayerAlreadyHasHq));
    }
}