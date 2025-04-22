using engine.Engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.Phases;

public class InventoryPhaseTest : BaseTest
{
    [SetUp]
    public void SetUp()
    {
        T(1).Owner = Players.Player1;
        T(2).Owner = Players.Player2;
        T(2).Units.AddArmies(5);
        
        Players.Player1.InfluencePoints = 40;
    }

    [Test]
    public void ItRejectsUsingAnItemBoughtInSameTurn()
    {
        List<Command> commands =
        [
            new BuyItemCommand
            {
                Issuer = Players.Player1,
                ItemType = () => Item.Dynamite,
            },
            new UseDynamite
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Target = T(2),
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Item is bought, but not used.
        // Note that this puts the IP to 21 (-20 from item, +1 from owned territory).
        Assert.Multiple(() =>
        {
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(21));
            Assert.That(Players.Player1.Inventory, Does.Contain(Item.Dynamite));
            Assert.That(T(2).Units.Armies, Is.EqualTo(5));
        });
    }
}