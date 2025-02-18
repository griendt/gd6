using gdvi.Engine;
using gdvi.Engine.Commands;
using gdvi.Models;

namespace tests;

public class CreateHqTest : BaseTest
{
    [Test]
    public void CreatingASingleHqIsValid()
    {
        var player = new Player { Id = 1 };
        List<CreateHq> commands = [ new() { Issuer = player, Origin = World.Territories[1] }];

        CreateHq.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }
    
    [Test]
    public void ItRejectsTwoPlayersBuildingHqsInSameTerritory()
    {
        var first = new Player { Id = 1 };
        var second = new Player { Id = 2 };
        
        List<CreateHq> commands = [
            new() { Issuer = first, Origin = World.Territories[1] },
            new() { Issuer = second, Origin = World.Territories[2] },
        ];

        CreateHq.Validate(commands, World);
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].IsRejected, Is.True);
            Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
            
            Assert.That(commands[1].IsRejected, Is.True);
            Assert.That(commands[1].Rejections, Has.Count.EqualTo(1));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(CommandRejection.BuildingHqInSameTerritoryAsAnotherPlayer));
            Assert.That(commands[1].Rejections[0].Reason, Is.EqualTo(CommandRejection.BuildingHqInSameTerritoryAsAnotherPlayer));
        });
    }
}