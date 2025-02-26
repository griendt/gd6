using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class UseDynamiteExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[1].Owner = Players.Player1;
    }

    [TestCase(1, 0)]
    [TestCase(47, 46)]
    [TestCase(0, 0)]
    public void ItKillsAUnit(int numBefore, int numAfter)
    {
        World.Territories[2].Units.AddArmies(numBefore);

        new UseDynamite { Issuer = Players.Player1, Origin = World.Territories[1], Target = World.Territories[2] }.Process(World);
        
        Assert.That(World.Territories[2].Units.Armies, Is.EqualTo(numAfter));
    }
    
    [TestCase(0)]
    [TestCase(1)]
    public void ItRendersTerritoryNeutralIfNoUnitsLeft(int numBefore)
    {
        World.Territories[2].Owner = Players.Player2;
        World.Territories[2].Units.AddArmies(numBefore);
        
        new UseDynamite { Issuer = Players.Player1, Origin = World.Territories[1], Target = World.Territories[2] }.Process(World);
        
        Assert.That(World.Territories[2].Owner, Is.Null);
    }

    [Test]
    public void ItDestroysABivouac()
    {
        World.Territories[2].Constructs.Add(Construct.Bivouac);
        
        new UseDynamite { Issuer = Players.Player1, Origin = World.Territories[1], Target = World.Territories[2] }.Process(World);
        
        Assert.That(World.Territories[2].Constructs, Does.Not.Contain(Construct.Bivouac));
    }
    
    [Test]
    public void ItTurnsFortressIntoRuin()
    {
        World.Territories[2].Constructs.Add(Construct.Fortress);
        
        new UseDynamite { Issuer = Players.Player1, Origin = World.Territories[1], Target = World.Territories[2] }.Process(World);
        
        Assert.That(World.Territories[2].Constructs, Does.Not.Contain(Construct.Fortress));
        Assert.That(World.Territories[2].Constructs, Does.Contain(Construct.Ruin));
    }
    
    [Test]
    public void ItLeavesRuinsIntact()
    {
        World.Territories[2].Constructs.Add(Construct.Ruin);
        
        new UseDynamite { Issuer = Players.Player1, Origin = World.Territories[1], Target = World.Territories[2] }.Process(World);
        
        Assert.That(World.Territories[2].Constructs, Does.Contain(Construct.Ruin));
    }
}