using gdvi.Engine.Commands;
using gdvi.Models;

namespace tests.CommandValidation;

public class CropSupplyValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[1].HqSettler = Players.Player1;
        World.Territories[1].Owner = Players.Player1;
    }

    [Test]
    public void ItAcceptsUsingCropSupplyIfInInventoryAndPresentOnMap()
    {
        var command = new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
        };
        Players.Player1.Inventory = [Item.CropSupply];

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.False);
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(5)]
    [Test(Description = "The amount of armies specified in a crop supply is limited, but it is allowed to use multiple supplies" +
                        "in one turn. This can multiply the total amount of armies spawned by crop supplies in one turn.")]
    public void ItAcceptsUsingMultipleCropSupplies(int numCrops)
    {
        var commands = Enumerable.Range(1, numCrops)
            .Select(i => new UseCropSupply
            {
                Issuer = Players.Player1,
                Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
            })
            .ToList();

        Players.Player1.Inventory = Enumerable.Range(1, numCrops).Select(Item (i) => Item.CropSupply).ToList();

        CommandValidator.Validate(commands, World);

        foreach (var command in commands) {
            Assert.That(command.IsRejected, Is.False);
        }
    }

    [Test]
    public void ItRejectsUsingCropSupplyIfAmountOfArmiesSpecifiedTooHigh()
    {
        var command = new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 6 } },
        };
        Players.Player1.Inventory = [Item.CropSupply];

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.True);
        Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.SpawningTooManyArmies));
    }

    [TestCase(0, 1)]
    [TestCase(1, 2)]
    [TestCase(3, 4)]
    [Test(Description = "Technically one command could be considered valid, but because this error is independent of other commands," +
                        "we want to reject all commands so that the user can fix their commands before the turn ends.")]
    public void ItRejectsUsingAnyCropSupplyIfNotEnoughItemsInInventory(int numCropsInInventory, int numCropsInCommands)
    {
        var commands = Enumerable.Range(1, numCropsInCommands).Select(i => new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
        }).ToList();

        Players.Player1.Inventory = Enumerable.Range(1, numCropsInInventory).Select(Item (i) => Item.CropSupply).ToList();

        CommandValidator.Validate(commands, World);

        foreach (var command in commands) {
            Assert.That(command.IsRejected, Is.True);
            Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.InsufficientAmountOfItems));
        }
    }

    [TestCase(1, 5)]
    [TestCase(3, 5)]
    [TestCase(11, 5)]
    [TestCase(12, 6)]
    [TestCase(13, 6)]
    [TestCase(15, 7)]
    public void ItAcceptsHigherQuantityInCropSupplyIfOwnsSufficientTerritories(int numOwnedTerritories, int maxArmiesForCropSupply)
    {
        foreach (var i in Enumerable.Range(1, numOwnedTerritories)) {
            World.Territories[i].Owner = Players.Player1;
        }
        Players.Player1.Inventory = [Item.CropSupply];

        var command = new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int> { { World.Territories[1].Id, maxArmiesForCropSupply } },
        };
        
        CommandValidator.Validate([command], World);
        
        Assert.That(command.IsRejected, Is.False);
    }
}