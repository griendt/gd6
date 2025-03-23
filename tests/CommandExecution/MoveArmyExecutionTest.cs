using engine;
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

    [TestCase(1)]
    [TestCase(2)]
    public void ItMovesAnArmy(int numMovements)
    {
        var command = new MoveArmy
        {
            Issuer = Players.Player1,
            Origin = World.Territories[1],
            Path = Enumerable.Range(2, numMovements).Select(i => World.Territories[i]).ToList(),
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

            // One unit is moved, and it is not moved to any intermediate territories,
            // only to the final one.
            Assert.That(command.Path.Sum(territory => territory.Units.Armies), Is.EqualTo(1));
            Assert.That(command.Path.Last().Units.Armies, Is.EqualTo(1));

            // All intermediate territories (if any) now belong to the issuer,
            // even though they are empty.
            command.Path.Each(territory => { Assert.That(territory.Owner, Is.EqualTo(command.Issuer)); });
        });
    }
}