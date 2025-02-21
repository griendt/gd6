using gdvi.Engine.Commands;

namespace tests.CommandExecution;

public class UseCropSupplyValidationTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[1].Owner = Players.Player1;
        World.Territories[1].Units.AddArmies(4);
    }

    [Test]
    public void ItAddsTheRequestedAmount()
    {
        var command = new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int>
            {
                { World.Territories[1].Id, 3 },
            },
        };

        command.Process(World);

        Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(7));
    }
}