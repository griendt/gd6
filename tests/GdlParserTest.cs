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

    [TestCase("Hq", typeof(CreateHq))]
    [TestCase("For", typeof(CreateFortress))]
    [TestCase("Biv", typeof(CreateBivouac))]
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
        var command = _parser.Commands.First() as MoveArmy;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(MoveArmy)));
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
        var command = _parser.Commands.First() as MoveArmy;

        Assert.Multiple(() =>
        {
            Assert.That(command!.Issuer.Name, Is.EqualTo(name));
            Assert.That(command, Is.InstanceOf(typeof(MoveArmy)));
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