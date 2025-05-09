using engine.Engine.Commands;

namespace tests.CommandExecution;

public class SpawnArmyExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        T(1).Owner = Players.Player1;
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(47)]
    public void ItSpawnsArmies(int numArmies)
    {
        var command = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Quantity = numArmies,
        };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(1).Units.IsEmpty, Is.False);
            Assert.That(T(1).Units.Armies, Is.EqualTo(numArmies));
        });
    }

    [TestCase(0, 1, 1)]
    [TestCase(0, 3, 3)]
    [TestCase(1, 1, 2)]
    [TestCase(1, 3, 4)]
    [TestCase(12, 34, 46)]
    public void ItAddsArmiesToExistingOnes(int numExisting, int numAdded, int numTotal)
    {
        var command = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Quantity = numAdded,
        };
        T(1).Units.AddArmies(numExisting);

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(1).Units.IsEmpty, Is.False);
            Assert.That(T(1).Units.Armies, Is.EqualTo(numTotal));
        });
    }

    [Test]
    public void ItClaimsOwnershipIfspawningInNeutralTerritoryNextToHq()
    {
        T(1).HqSettler = Players.Player1;

        var command = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = T(2),
            Quantity = 1,
        };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(2).Units.Armies, Is.EqualTo(1));
        });
    }
}