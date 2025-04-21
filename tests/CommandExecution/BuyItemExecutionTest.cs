using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class BuyItemExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).Owner = Players.Player1;
    }

    [TestCase(Item.CropSupply)]
    [TestCase(Item.Dynamite)]
    [TestCase(Item.ToxicWaste)]
    public void ItAddsItemsToInventory(Item itemType)
    {
        var command = new BuyItemCommand { Issuer = Players.Player1, ItemType = () => itemType };
        
        command.Process(World);

        Assert.That(Players.Player1.Inventory, Does.Contain(itemType));
    }
}