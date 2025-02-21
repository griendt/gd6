using gdvi.Engine.Commands;

namespace tests.CommandExecution;

public class SpawnArmyExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[1].Owner = Players.Player1;
    }
    
    [Test]
    public void ItSpawnsAnArmy()
    {
        var command = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = World.Territories[1],
            Quantity = 1,
        };
        
        command.Process(World);
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[1].Owner, Is.EqualTo(Players.Player1));
            Assert.That(World.Territories[1].Units.IsEmpty, Is.False);
            Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(1));
        });
    }
    
    [Test]
    public void ItSpawnsMultipleArmies()
    {
        var command = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = World.Territories[1],
            Quantity = 3,
        };
        
        command.Process(World);
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[1].Owner, Is.EqualTo(Players.Player1));
            Assert.That(World.Territories[1].Units.IsEmpty, Is.False);
            Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(3));
        });
    }
    
    [Test]
    public void ItAddsArmiesToExistingOnes()
    {
        var command = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = World.Territories[1],
            Quantity = 3,
        };
        World.Territories[1].Units.AddArmies(6);
        
        command.Process(World);
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[1].Owner, Is.EqualTo(Players.Player1));
            Assert.That(World.Territories[1].Units.IsEmpty, Is.False);
            Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(9));
        });
    }
}