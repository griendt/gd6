using engine;
using engine.Engine;
using engine.Engine.Commands;

namespace tests;

public class MovementPhaseTest : BaseTest
{
    [SetUp]
    public void SetUpForSkirmish()
    {
        World.Territories[1].Owner = Players.Player1;
        World.Territories[1].Units.AddArmies(5);

        World.Territories[3].Owner = Players.Player2;
        World.Territories[3].Units.AddArmies(5);

        World.AddBorder(World.Territories[1], World.Territories[2]);
        World.AddBorder(World.Territories[2], World.Territories[3]);
    }

    [Test]
    public void ItDetectsASimpleSkirmish()
    {
        List<Command> commands =
        [
            new MoveArmy
            {
                Issuer = Players.Player1,
                Origin = World.Territories[1],
                Path = [World.Territories[1], World.Territories[2]],
            },
            new MoveArmy
            {
                Issuer = Players.Player2,
                Origin = World.Territories[3],
                Path = [World.Territories[3], World.Territories[2]],
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Processed as a skirmish
        Assert.Multiple(() =>
        {
            commands.OfType<MoveArmy>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(4));
            Assert.That(World.Territories[3].Units.Armies, Is.EqualTo(4));

            Assert.That(World.Territories[2].Units.Armies, Is.EqualTo(0));
            Assert.That(World.Territories[2].IsNeutral);
        });
    }
}