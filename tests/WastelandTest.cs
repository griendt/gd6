using gd6.Engine;
using gd6.Models;

namespace tests;

public class WastelandTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[3].Owner = Players.Player3;
        World.Territories[3].IsWasteland = true;
    }
    
    [TestCase(1, 0, true)]
    [TestCase(3, 2, false)]
    [TestCase(47, 46, false)]
    [TestCase(0, 0, true)]
    public void ItRemovesArmyFromWasteland(int numBefore, int numAfter, bool shouldBeNeutralized)
    {
        World.Territories[3].Units.AddArmies(numBefore);
        
        new Turn(World).Process();
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[3].Units.Armies, Is.EqualTo(numAfter));
            Assert.That(World.Territories[3].Owner, shouldBeNeutralized ? Is.Null : Is.EqualTo(Players.Player3));
        });
    }
}