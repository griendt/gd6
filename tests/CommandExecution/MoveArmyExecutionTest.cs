using engine.Engine.Commands;

namespace tests.CommandExecution;

public class MoveArmyExecutionTest : BaseTest
{
    [SetUp]
    public void SetUpOwners()
    {
        World.Territories[1].Owner = Players.Player1;
        World.Territories[1].Units.AddArmies(5);
    }

    [Test]
    public void ItMovesAnArmy()
    {
        var command = new MoveArmy
        {
            Issuer = Players.Player1,
            Origin = World.Territories[1],
            Path = [World.Territories[2]],
        };

        command.Process(World);

        // TODO: command processing may depend on other commands (conflicting moves).
        //  Extend the command with a "conflicting commands" property and fill it
        //  e.g. during validation. Then each command in a command group must be
        //  executed at once, so that when one command is executed, all of them are.
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[2].Owner, Is.EqualTo(Players.Player1));
            Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(4));
            Assert.That(World.Territories[2].Units.Armies, Is.EqualTo(1));
        });
    }
}