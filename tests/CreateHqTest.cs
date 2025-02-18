using gdvi.Engine.Commands;

namespace tests;

public class CreateHqTest : BaseTest
{
    [Test]
    public void CreatingASingleHqIsValid()
    {
        List<CreateHq> commands = [ new() { Issuer = Players.Player1, Origin = World.Territories[1] }];

        CreateHq.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }
    
    [Test]
    public void ItRejectsTwoPlayersBuildingHqsInSameTerritory()
    {
        List<CreateHq> commands = [
            new() { Issuer = Players.Player1, Origin = World.Territories[1] },
            new() { Issuer = Players.Player2, Origin = World.Territories[1] },
        ];

        CreateHq.Validate(commands, World);
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
            Assert.That(commands[1].Rejections, Has.Count.EqualTo(1));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
            Assert.That(commands[1].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
        });
    }
    
    [Test]
    public void ItRejectsTwoPlayersBuildingHqsInAdjacentTerritories()
    {
        List<CreateHq> commands = [
            new() { Issuer = Players.Player1, Origin = World.Territories[1] },
            new() { Issuer = Players.Player2, Origin = World.Territories[2] },
        ];

        CreateHq.Validate(commands, World);
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
            Assert.That(commands[1].Rejections, Has.Count.EqualTo(1));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
            Assert.That(commands[1].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToAnotherPlayerBuildingHq));
        });
    }
    
    [Test]
    public void ItRejectsBuildingHqOnTopOfExistingHq()
    {
        World.Territories[1].HqSettler = Players.Player1;
        
        List<CreateHq> commands = [
            new() { Issuer = Players.Player2, Origin = World.Territories[1] },
        ];

        CreateHq.Validate(commands, World);
        
        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToExistingHq));
    }
    
    [Test]
    public void ItRejectsBuildingHqNextToExistingHq()
    {
        World.Territories[1].HqSettler = Players.Player1;
        
        List<CreateHq> commands = [
            new() { Issuer = Players.Player2, Origin = World.Territories[2] },
        ];

        CreateHq.Validate(commands, World);
        
        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToExistingHq));
    }
    
    [Test]
    public void ItRejectsBuildingHqWithBorderAdjacentToExistingHq()
    {
        World.Territories[1].HqSettler = Players.Player1;
        
        List<CreateHq> commands = [
            new() { Issuer = Players.Player2, Origin = World.Territories[3] },
        ];

        CreateHq.Validate(commands, World);
        
        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqTooCloseToExistingHq));
    }
    
    [Test]
    public void ItAllowsBuildingHqAtDistance3OfExistingHq()
    {
        World.Territories[1].HqSettler = Players.Player1;
        
        List<CreateHq> commands = [
            new() { Issuer = Players.Player2, Origin = World.Territories[4] },
        ];

        CreateHq.Validate(commands, World);

        Assert.That(commands[0].IsRejected, Is.False);
    }
    
    [Test]
    public void ItRejectsBuildingHqOnOccupiedTerritory()
    {
        World.Territories[1].Owner = Players.Player1;
        
        List<CreateHq> commands = [
            new() { Issuer = Players.Player2, Origin = World.Territories[1] },
        ];

        CreateHq.Validate(commands, World);

        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqOnOccupiedTerritory));
    }
    
    [Test]
    public void ItRejectsBuildingMultipleHqs()
    {
        List<CreateHq> commands = [
            new() { Issuer = Players.Player1, Origin = World.Territories[1] },
            new() { Issuer = Players.Player1, Origin = World.Territories[4] },
        ];

        CreateHq.Validate(commands, World);

        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
            Assert.That(commands[1].Rejections, Has.Count.EqualTo(1));
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingMultipleHqs));
            Assert.That(commands[1].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingMultipleHqs));
        });
    }
    
    [Test]
    public void ItRejectsBuildingHqIfPlayerAlreadyHasHq()
    {
        World.Territories[1].HqSettler = Players.Player1;
        
        List<CreateHq> commands = [
            new() { Issuer = Players.Player1, Origin = World.Territories[4] },
        ];

        CreateHq.Validate(commands, World);

        Assert.That(commands[0].Rejections, Has.Count.EqualTo(1));
        Assert.That(commands[0].Rejections[0].Reason, Is.EqualTo(RejectReason.BuildingHqWhenPlayerAlreadyHasHq));
    }
}