using engine.Engine.Commands;
using engine.Models;
using JetBrains.Annotations;

namespace tests.CommandValidation;

public class BuyItemValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).HqSettler = Players.Player1;
        T(1).Owner = Players.Player1;
    }

    [TestCase(15, Item.Dynamite, true)]
    [TestCase(15, Item.ToxicWaste, true)]
    [TestCase(20, Item.Dynamite, false)]
    [TestCase(25, Item.CropSupply, true)]
    [TestCase(30, Item.CropSupply, false)]
    public void ValidateEnoughInfluencePointsForOneItem(int numInfluencePoints, Item itemType, bool shouldBeRejected)
    {
        Players.Player1.InfluencePoints = numInfluencePoints;
        var command = new BuyItemCommand { Issuer = Players.Player1, ItemType = () => itemType };
        
        CommandValidator.Validate([command], World);
        
        Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
        if (shouldBeRejected) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
        }
    }

    [TestCase(45, true)]
    [TestCase(50, false)]
    public void ValidateEnoughInfluencePointsForMixedItems(int numInfluencePoints, bool shouldBeRejected)
    {
        Players.Player1.InfluencePoints = numInfluencePoints;
        
        List<BuyItemCommand> commands =
        [
            new() { Issuer = Players.Player1, ItemType = () => Item.Dynamite },
            new() { Issuer = Players.Player1, ItemType = () => Item.CropSupply },
        ];
        
        CommandValidator.Validate(commands, World);
        
        commands.ForEach(command =>
        {
            Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
            if (shouldBeRejected) {
                Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
            }
        });
    }
}