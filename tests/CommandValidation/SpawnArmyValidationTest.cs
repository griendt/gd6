using engine.Engine.Commands;

namespace tests.CommandValidation;

public class SpawnArmyValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).HqSettler = Players.Player1;
        T(1).Owner = Players.Player1;
    }

    [Test]
    public void ItAcceptsSpawningOnOwnHqIfOwned()
    {
        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }

    [Test(Description = "The issuer owns the HQ that was build by another player. This is OK.")]
    public void ItAcceptsSpawningOnOtherHqIfOwned()
    {
        T(1).HqSettler = Players.Player2;
        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }

    [Test(Description = "The HQ was settled by the issuer, but another player is occupying it." +
                        "The player has to spawn according to the 'longest concurrent occupation' rule.")]
    public void ItRejectsSpawningOnOwnHqIfNotOwned()
    {
        T(1).Owner = Players.Player2;
        T(4).Owner = Players.Player1;

        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.True);
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.SpawningNotInLongestConcurrentlyOccupyingTerritory));
    }

    [Test]
    public void ItRejectsSpawningTooManyArmies()
    {
        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = 3 }];

        CommandValidator.Validate(commands, World);

        Assert.Multiple(() =>
        {
            Assert.That(World.NumberOfTerritories(Players.Player1), Is.EqualTo(1));
            Assert.That(commands[0].IsRejected, Is.True);
        });

        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.SpawningTooManyArmies));
    }

    [Test]
    public void ItRejectsSpawningNegativeArmies()
    {
        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(1), Quantity = -3 }];

        CommandValidator.Validate(commands, World);

        Assert.Multiple(() =>
        {
            Assert.That(World.NumberOfTerritories(Players.Player1), Is.EqualTo(1));
            Assert.That(commands[0].IsRejected, Is.True);
        });

        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.SpawningNegativeArmies));
    }

    [Test(Description = "You may spawn next to an owned HQ, even if you do not own that neighbour." +
                        "Note: This does not make the neighbour automatically yours.")]
    public void ItAcceptsSpawningAdjacentToOwnedHqEvenIfTerritoryNotOwned()
    {
        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(2), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.Multiple(() =>
        {
            Assert.That(World.NumberOfTerritories(Players.Player1), Is.EqualTo(1));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(commands[0].IsRejected, Is.False);
        });
    }

    [Test]
    public void ItRejectsSpawningTooFarFromOwnHq()
    {
        List<SpawnArmy> commands = [new() { Issuer = Players.Player1, Origin = T(3), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.Multiple(() =>
        {
            Assert.That(World.NumberOfTerritories(Players.Player1), Is.EqualTo(1));
            Assert.That(commands[0].IsRejected, Is.True);
        });

        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.SpawningTooFarFromOwnHq));
    }

    [Test]
    public void ItAcceptsSpawningInLongestConcurrentlyOccupyingTerritoryIfNotOwningHq()
    {
        T(2).Owner = Players.Player2;
        T(2).Loyalty = 3;
        T(3).Owner = Players.Player2;
        T(3).Loyalty = 4;

        List<SpawnArmy> commands = [new() { Issuer = Players.Player2, Origin = T(3), Quantity = 1 }];

        CommandValidator.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }

    [Test]
    public void ItRejectsSpawningIfNoTerritoriesOwned()
    {
        var command = new SpawnArmy { Issuer = Players.Player2, Origin = T(1), Quantity = 1 };

        CommandValidator.Validate([command], World);

        Assert.Multiple(() =>
        {
            Assert.That(World.NumberOfTerritories(Players.Player2), Is.EqualTo(0));
            Assert.That(command.IsRejected, Is.True);
        });
        Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.NoValidSpawnLocations));
    }

    [Test(Description = "Even though the distant land is longest owned, the player must spawn on or near their HQ.")]
    public void ItRejectsSpawningInFarAwayLongestConcurrentlyOccupyingTerritoryIfOwnsHq()
    {
        T(4).Owner = Players.Player1;
        T(1).Loyalty = 1;
        T(4).Loyalty = 3;
        var command = new SpawnArmy { Issuer = Players.Player1, Origin = T(4), Quantity = 1 };

        CommandValidator.Validate([command], World);

        Assert.Multiple(() =>
        {
            Assert.That(World.NumberOfTerritories(Players.Player1), Is.EqualTo(2));
            Assert.That(command.IsRejected, Is.True);
        });

        Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.SpawningTooFarFromOwnHq));
    }
}