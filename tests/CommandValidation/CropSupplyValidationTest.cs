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
        Players.Player1.Inventory = [new CropSupply()];

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.False);
    }

    [Test]
    public void ItRejectsUsingCropSupplyIfNotInInventory()
    {
        var command = new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
        };

        Players.Player1.Inventory = [];

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.True);
        Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.InsufficientAmountOfItems));
    }

    [Test(Description = "Technically one command could be considered valid, but because this error is independent of other commands," +
                        "we want to reject all commands so that the user can fix their commands before the turn ends.")]
    public void ItRejectsUsingAnyCropSupplyIfNotEnoughItemsInInventory()
    {
        List<UseCropSupply> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
            },
            new()
            {
                Issuer = Players.Player1,
                Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
            },
            new()
            {
                Issuer = Players.Player1,
                Quantities = new Dictionary<int, int> { { World.Territories[1].Id, 5 } },
            },
        ];
        Players.Player1.Inventory = [new CropSupply()];

        CommandValidator.Validate(commands, World);

        foreach (var command in commands) {
            Assert.That(command.IsRejected, Is.True);
            Assert.That(command.Rejections[0].Reason, Is.EqualTo(RejectReason.InsufficientAmountOfItems));
        }
    }
}