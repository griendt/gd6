using engine;
using engine.Engine.Commands;
using gdl.Exceptions;

namespace tests.GdlParser;

public class GdlParserTest : BaseTest
{
    private gdl.GdlParser _parser;

    [SetUp]
    public void SetUpParser()
    {
        _parser = new gdl.GdlParser(World);
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
    public void ItParsesACreateHqOrder()
    {
        _parser.Parse($"Set {Players.Player1.Name}\nCon {World.Territories.First().Value.Id} Hq");

        Assert.That(_parser.Commands, Has.Count.EqualTo(1));

        var command = _parser.Commands.First() as CreateHq;
        Assert.That(command, Is.InstanceOf(typeof(CreateHq)));

        Assert.Multiple(() =>
        {
            Assert.That(command.Issuer, Is.EqualTo(Players.Player1));
            Assert.That(command.Origin, Is.EqualTo(World.Territories.First().Value));
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
}