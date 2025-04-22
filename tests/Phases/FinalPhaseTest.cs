using engine;
using engine.Engine;
using engine.Models;

namespace tests.Phases;

public class FinalPhaseTest : BaseTest
{

    [TestCase(1)]
    [TestCase(4)]
    [TestCase(8)]
    public void ItAddsInfluencePointsPerTerritory(int numTerritories)
    {
        Enumerable.Range(1, numTerritories).Each(i => T(i).Owner = Players.Player1);
        new Turn { World = World, Commands = [] }.Process();

        Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(numTerritories));
    }

    [TestCase(1)]
    [TestCase(2)]
    [TestCase(3)]
    public void ItAddsInfluencePointsPerHq(int numHqs)
    {
        Enumerable.Range(1, numHqs).Each(i =>
        {
            T(i * 3).HqSettler = Players.Player1;
            T(i * 3).Owner = Players.Player1;
        });

        new Turn { World = World, Commands = [] }.Process();

        // 1 IP for owning the territory and an additional 6 because it has an HQ.
        Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(numHqs * 7));
    }

    [TestCase(false, false, false)]
    [TestCase(false, true, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    public void ItProcessesBivouac(bool isOwned, bool isWasteland, bool shouldGetBivouacBonus)
    {
        T(1).Owner = isOwned ? Players.Player1 : null;
        T(1).IsWasteland = isWasteland;
        T(1).Constructs.Add(Construct.Bivouac);
        
        new Turn { World = World, Commands = [] }.Process();

        Assert.That(T(1).Units.Armies, Is.EqualTo(shouldGetBivouacBonus ? 1 : 0));
    }

    [Test]
    public void ItIncreasesLoyaltyOfOwnedTerritories()
    {
        T(1).Owner = Players.Player1;
        T(2).Owner = Players.Player1;
        T(2).Units.AddArmies(5);
        
        new Turn { World = World, Commands = [] }.Process();

        Assert.Multiple(() =>
        {
            // Regardless of whether there are any armies stationed or not
            Assert.That(T(1).Loyalty, Is.EqualTo(1));
            Assert.That(T(2).Loyalty, Is.EqualTo(1));
            Assert.That(T(3).Loyalty, Is.EqualTo(0));
        });
    }

    [TestCase(0, 0, 2)]
    [TestCase(4, 4, 2)]
    [TestCase(9, 0, 3)]
    [TestCase(99, 0, 12)]
    [TestCase(6, 3, 3)]
    [TestCase(36, 43, 10)]
    public void ItAddsInfluencePointsPer10Armies(int numArmiesInT1, int numArmiesInT2, int expectedInfluencePoints)
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(numArmiesInT1);
        T(1).Constructs.Add(Construct.Bivouac);
        T(2).Owner = Players.Player1;
        T(2).Units.AddArmies(numArmiesInT2);
        
        new Turn { World = World, Commands = [] }.Process();
        Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(expectedInfluencePoints));
    }
}