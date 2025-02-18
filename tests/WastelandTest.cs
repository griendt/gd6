using gdvi.Engine;
using gdvi.Models;

namespace tests;

public class WastelandTest : BaseTest
{
    [Test]
    public void ItRemovesArmyFromWasteland()
    {
        World.Territories[3].Units.AddArmies(4);
        World.Territories[3].IsWasteland = true;
        new Turn(World).Process();
        
        Assert.That(World.Territories[3].Units.Armies, Is.EqualTo(3));
    }
    
    [Test]
    public void ItDoesNotMakeTerritoryNeutralIfLastUnitIsRemovedByWasteland()
    {
        World.Territories[3].Owner = new Player
        {
            Id = 1,
            Inventory = [],
            Hq = World.Territories[3],
            Color = "#f00",
        };
        
        World.Territories[3].Units.AddArmy();
        World.Territories[3].IsWasteland = true;
        
        new Turn(World).Process();
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[3].Units.Armies, Is.EqualTo(0));
            Assert.That(World.Territories[3].Owner, Is.Not.Null);
        });
    }
}