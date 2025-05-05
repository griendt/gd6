using engine;
using engine.Engine.Commands;
using engine.Models;
using gdl;
using gdl.Exceptions;

namespace tests;

public class GdlParserTest : BaseTest
{
    private GdlParser _parser;

    [SetUp]
    public void SetUpParser()
    {
        _parser = new GdlParser(World);
    }

    [Test]
    public void ItNoopsOnEmptyCommandList()
    {
        _parser.Parse("");

        Assert.That(_parser.Commands, Is.Empty);
    }

    [Test]
    public void ItNoopsOnCommandListWithOnlyPlayerNames()
    {
        _parser.Parse($"Set {Players.Player1.Name}");

        Assert.That(_parser.Commands, Is.Empty);
    }

    [Test]
    public void ItRejectsCommandSetWithUnknownPlayerName()
    {
        Assert.Throws<UnknownPlayerException>(() => { _parser.Parse("Set UnknownPlayer"); });
        Assert.That(_parser.Commands, Is.Empty);
    }

    [Test]
    public void ItRejectsInitEndIfNotInitStarted()
    {
        Assert.Throws<Exception>(() => _parser.Parse("InitEnd"));
    }

    [TestCase("AddPlayer Aluce #f00")]
    [TestCase("SetNumTerritories 5")]
    [TestCase("SetNumTerritories 5\nSetBoundaries 1,2;3,4")]
    public void ItRejectsInitCommandsIfNotInitStarted(string command)
    {
        World.Territories = [];
        World.TerritoryBorders = [];

        Assert.Throws<CommandSetNotInitialized>(() => _parser.Parse(command));
        Assert.DoesNotThrow(() => _parser.Parse($"InitStart\n{command}"));
    }

    [TestCase("#f00", false)]
    [TestCase("#F80", false)]
    [TestCase("#a8F", false)]
    [TestCase("#ff0000", true)]
    [TestCase("#FF0000", true)]
    [TestCase("f00", true)]
    [TestCase("#g00", true)]
    public void ItChecksPlayerColors(string color, bool shouldExpectError)
    {
        var gdl = $"InitStart\nAddPlayer Aluce {color}";
        if (shouldExpectError) {
            Assert.Throws<InvalidColorException>(() => _parser.Parse(gdl));
        }
        else {
            Assert.DoesNotThrow(() => _parser.Parse(gdl));
        }
    }

    [Test]
    public void ItAddsPlayers()
    {
        World.Players = [];

        _parser.Parse("InitStart\nAddPlayer Aluce #f00\nAddPlayer Psycho17 #0f0");

        Assert.That(World.Players.Count, Is.EqualTo(2));
        foreach (var player in World.Players) {
            var color = player.Name switch
            {
                "Aluce" => "#f00",
                "Psycho17" => "#0f0",
                _ => throw new ArgumentOutOfRangeException(),
            };

            Assert.That(player.Colour, Is.EqualTo(color));
        }
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(6)]
    public void ItAddsTerritories(int numTerritories)
    {
        World.Territories = [];

        _parser.Parse($"InitStart\nSetNumTerritories {numTerritories}");

        Assert.That(World.Territories, Has.Count.EqualTo(numTerritories));
    }

    [Test]
    public void ItRejectsBoundariesFromUnknownTerritories()
    {
        Assert.Throws<UnknownTerritoryException>(() => _parser.Parse("InitStart\nSetBoundaries 21,22"));
    }

    [TestCase("1,2")]
    [TestCase("1,2;3,4")]
    [TestCase("1,2;2,3")]
    public void ItSetsTerritoryBoundaries(string boundaries)
    {
        _parser.Parse($"InitStart\nSetBoundaries {boundaries}");

        foreach (var boundary in boundaries.Split(";")) {
            var items = boundary.Split(",");

            Assert.Multiple(() =>
            {
                Assert.That(World.TerritoryBorders, Does.ContainKey(int.Parse(items[0])));
                Assert.That(World.TerritoryBorders, Does.ContainKey(int.Parse(items[1])));
            });

            Assert.Multiple(() =>
            {
                Assert.That(World.TerritoryBorders[int.Parse(items[0])], Does.Contain(int.Parse(items[1])));
                Assert.That(World.TerritoryBorders[int.Parse(items[1])], Does.Contain(int.Parse(items[0])));
            });
        }
    }

    [TestCase("21")]
    [TestCase("Banana")]
    public void ItRejectsInvalidTerritoryIdInCoordinateCommand(string territoryId)
    {
        Assert.Throws<UnknownTerritoryException>(() => _parser.Parse("InitStart\nSetCoordinates 21 1,2;3,4;5,6"));
    }

    [TestCase("1,2,3;4,5,6;7,8,9", Description = "Coordinates should be 2-dimensional")]
    [TestCase("1,2;3,4", Description = "Coordinates should have length at least 3 to be displayable")]
    [TestCase("a,2;3,4;5,6", Description = "Coordinates must not contain letters")]
    [TestCase("1.0,2.0;3.0,4;5,6.0", Description = "Coordinates must be integral")]
    [TestCase("1,2;-3,4;5,6", Description = "Coordinates must be non-negative")]
    public void ItRejectsInvalidCoordinates(string command)
    {
        Assert.Throws<InvalidArgumentException>(() => _parser.Parse($"InitStart\nSetCoordinates 1 {command}"));
    }

    [Test]
    public void ItAddsCoordinates()
    {
        _parser.Parse("InitStart\nSetCoordinates 1 0,0;10,10;20,30;0,40");

        Assert.That(World.Territories[1].Coordinates, Has.Count.EqualTo(4));
        Assert.That(World.Territories[1].Coordinates,
        Is.EqualTo([
            (0, 0),
            (10, 10),
            (20, 30),
            (0, 40),
        ]));
    }

    [Test]
    public void ItAddsEndOfTurnCommand()
    {
        _parser.Parse($"Set {Players.Player1.Name}\nEnd");
        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        Assert.That(_parser.Commands[0], Is.InstanceOf<EndOfTurnCommand>());
    }

    [TestCase("Hq", typeof(CreateHq))]
    [TestCase("Tow", typeof(CreateWatchtower))]
    [TestCase("Biv", typeof(CreateBivouac))]
    [TestCase("Int", typeof(CreateIntelligence))]
    [TestCase("Lib", typeof(CreateLibrary))]
    public void ItParsesACreateConstructOrder(string identifier, Type orderType)
    {
        _parser.Parse($"Set {Players.Player1.Name}\nCon {World.Territories.First().Value.Id} {identifier}");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));

        var command = _parser.Commands.First();
        Assert.That(command, Is.InstanceOf(orderType));

        Assert.Multiple(() =>
        {
            Assert.That(command.Issuer, Is.EqualTo(Players.Player1));
            Assert.That((command as IHasOrigin)!.Origin, Is.EqualTo(World.Territories.First().Value));
        });
    }

    [Test]
    public void ItRejectsConstructionOrderBeforeSettingIssuer()
    {
        Assert.Throws<CommandSetNotInitialized>(() => _parser.Parse("Con 1 Hq"));
    }

    [Test]
    public void ItRejectsACreateHqOrderWithUnknownOrigin()
    {
        Assert.Throws<UnknownTerritoryException>(() => { _parser.Parse($"Set {Players.Player1.Name}\nCon UnknownTerritory Hq"); });
    }

    [Test]
    public void ItRejectsUnknownCommandTypes()
    {
        Assert.Throws<UnknownCommandType>(() => _parser.Parse($"Set {Players.Player1.Name}\nFoo Bar Baz"));
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(12)]
    public void ItParsesConstructArmyOrder(int quantity)
    {
        var name = Players.Player1.Name;
        var territoryId = World.Territories.First().Value.Id;
        _parser.Parse($"Set {name}\nCon {territoryId} {quantity}A");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as SpawnArmy;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(SpawnArmy)));
            Assert.That(command!.Origin.Id, Is.EqualTo(territoryId));
            Assert.That(command.Quantity, Is.EqualTo(quantity));
        });
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(12)]
    public void ItParsesPromoteToCavalryOrder(int quantity)
    {
        var name = Players.Player1.Name;
        var territoryId = World.Territories.First().Value.Id;
        _parser.Parse($"Set {name}\nCon {territoryId} {quantity}C");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as PromoteArmy;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(PromoteArmy)));
            Assert.That(command!.Origin.Id, Is.EqualTo(territoryId));
            Assert.That(command.Quantity, Is.EqualTo(quantity));
            Assert.That(command.UnitType, Is.EqualTo(Unit.Cavalry));
        });
    }

    [TestCase(1)]
    [TestCase(3)]
    [TestCase(12)]
    public void ItParsesPromoteToHeavyOrder(int quantity)
    {
        var name = Players.Player1.Name;
        var territoryId = World.Territories.First().Value.Id;
        _parser.Parse($"Set {name}\nCon {territoryId} {quantity}H");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as PromoteArmy;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(PromoteArmy)));
            Assert.That(command!.Origin.Id, Is.EqualTo(territoryId));
            Assert.That(command.Quantity, Is.EqualTo(quantity));
            Assert.That(command.UnitType, Is.EqualTo(Unit.Heavy));
        });
    }

    [Test]
    public void ItParsesMultipleConstructArmyOrders()
    {
        var name = Players.Player1.Name;
        _parser.Parse($"Set {name}\nCon 1 1A\nCon 2 1A\nCon 3 1A");

        Assert.That(_parser.Commands, Has.Count.EqualTo(3));

        foreach (var (index, rawCommand) in _parser.Commands.Enumerate()) {
            var command = rawCommand as SpawnArmy;
            Assert.Multiple(() =>
            {
                Assert.That(command!.Issuer.Name, Is.EqualTo(name));
                Assert.That(command, Is.InstanceOf(typeof(SpawnArmy)));
                Assert.That(command.Origin.Id, Is.EqualTo(index + 1));
                Assert.That(command.Quantity, Is.EqualTo(1));
            });
        }
    }

    [Test]
    public void ItParsesBasicMoveArmyOrder()
    {
        var name = Players.Player1.Name;
        _parser.Parse($"Set {name}\nMov 1→2 1A");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as MoveUnit;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(MoveUnit)));
            Assert.That(command.Origin.Id, Is.EqualTo(1));
            Assert.That(command.Path.Select(territory => territory.Id).ToList(), Is.EqualTo([1, 2]));
        });
    }

    [Test]
    public void ItParsesALongerPathOfMoves()
    {
        var name = Players.Player1.Name;
        _parser.Parse($"Set {name}\nMov 1→2→3→4 1A");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as MoveUnit;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(MoveUnit)));
            Assert.That(command.Origin.Id, Is.EqualTo(1));
            Assert.That(command.Path.Select(territory => territory.Id).ToList(), Is.EqualTo([1, 2, 3, 4]));
        });
    }

    [TestCase("Dyn", Item.Dynamite)]
    [TestCase("Crp", Item.CropSupply)]
    [TestCase("Tox", Item.ToxicWaste)]
    public void ItParsesABuyOrder(string identifier, Item item)
    {
        var name = Players.Player1.Name;
        _parser.Parse($"Set {name}\nBuy {identifier}");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as BuyItemCommand;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(BuyItemCommand)));
            Assert.That(command.ItemType(), Is.EqualTo(item));
        });
    }

    [Test]
    public void ItParsesAUseDynamiteOrder()
    {
        var name = Players.Player1.Name;
        _parser.Parse($"Set {name}\nUse 1→2 Dyn");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as UseDynamite;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(UseDynamite)));
            Assert.That(command.Origin.Id, Is.EqualTo(1));
            Assert.That(command.Target.Id, Is.EqualTo(2));
        });
    }

    [Test]
    public void ItRejectsAUseDynamiteOrderIfFirstArgumentNotSimplePath()
    {
        var name = Players.Player1.Name;

        Assert.Multiple(() =>
        {
            Assert.Throws<InvalidPathLengthException>(() => _parser.Parse($"Set {name}\nUse 1 Dyn"));
            Assert.Throws<InvalidPathLengthException>(() => _parser.Parse($"Set {name}\nUse 1→2→3 Dyn"));
        });
    }

    [Test]
    public void ItParsesAUseCropSupplyOrder()
    {
        var name = Players.Player1.Name;

        _parser.Parse($"Set {name}\nUse 1:3,2:2 Crp");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));
        var command = _parser.Commands.First() as UseCropSupply;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(UseCropSupply)));
        });

        Assert.That(command.Quantities.Keys.ToList(), Is.EqualTo([1, 2]));

        Assert.Multiple(() =>
        {
            Assert.That(command.Quantities[1], Is.EqualTo(3));
            Assert.That(command.Quantities[2], Is.EqualTo(2));
        });
    }
}