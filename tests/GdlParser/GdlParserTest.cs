using engine.Engine.Commands;
using gdl.Exceptions;

namespace tests.GdlParser;

public class GdlParserTest : BaseTest
{
    public gdl.GdlParser Parser;

    [SetUp]
    public void SetUpParser()
    {
        Parser = new gdl.GdlParser(World);
    }

    [Test]
    public void ItNoopsOnEmptyCommandList()
    {
        Parser.Parse("");

        Assert.That(Parser.Commands, Is.Empty);
    }

    [Test]
    public void ItNoopsOnCommandListWithOnlyPlayerNames()
    {
        Parser.Parse($"Set {Players.Player1.Name}");

        Assert.That(Parser.Commands, Is.Empty);
    }

    [Test]
    public void ItRejectsCommandSetWithUnknownPlayerName()
    {
        Assert.Throws<UnknownPlayerException>(() => { Parser.Parse("Set UnknownPlayer"); });
        Assert.That(Parser.Commands, Is.Empty);
    }

    [Test]
    public void ItParsesACreateHqOrder()
    {
        Parser.Parse($"Set {Players.Player1.Name}\nCon {World.Territories.First().Value.Id} Hq");

        Assert.That(Parser.Commands, Has.Count.EqualTo(1));

        var command = Parser.Commands.First() as CreateHq;
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
        Assert.Throws<CommandSetNotInitialized>(() => Parser.Parse("Con 1 Hq"));
    }

    [Test]
    public void ItRejectsACreateHqOrderWithUnknownOrigin()
    {
        Assert.Throws<UnknownTerritoryException>(() => { Parser.Parse($"Set {Players.Player1.Name}\nCon UnknownTerritory Hq"); });
    }
}