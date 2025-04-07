using engine.Engine.Commands;

namespace tests.CommandExecution;

public class UseCropSupplyExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).Owner = Players.Player1;
    }

    [TestCase(0, 3, 3)]
    [TestCase(1, 3, 4)]
    [TestCase(4, 3, 7)]
    [TestCase(4, 47, 51)]
    public void ItAddsTheRequestedAmount(int numExisting, int numAdded, int numTotal)
    {
        var command = new UseCropSupply
        {
            Issuer = Players.Player1,
            Quantities = new Dictionary<int, int>
            {
                { T(1).Id, numAdded },
            },
        };
        T(1).Units.AddArmies(numExisting);


        command.Process(World);

        Assert.That(T(1).Units.Armies, Is.EqualTo(numTotal));
    }
}