using engine;
using engine.Engine;
using engine.Engine.Commands;

namespace tests.Phases;

public class MovementPhaseTest : BaseTest
{
    [SetUp]
    public void SetUpForSkirmish()
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(5);

        T(3).Owner = Players.Player2;
        T(3).Units.AddArmies(5);

        World.AddBorder(T(1), T(2));
        World.AddBorder(T(2), T(3));
        World.AddBorder(T(1), T(3));
    }

    [Test]
    public void ItDetectsASimpleSkirmish()
    {
        List<Command> commands =
        [
            new MoveArmy
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveArmy
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(3), T(2)],
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Processed as a skirmish
        Assert.Multiple(() =>
        {
            commands.OfType<MoveArmy>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));

            Assert.That(T(2).Units.Armies, Is.EqualTo(0));
            Assert.That(T(2).IsNeutral);
        });
    }

    [Test]
    public void ItResolvesMutualInvasionAsSkirmish()
    {
        T(2).Owner = Players.Player2;
        T(2).Units.AddArmies(5);

        List<Command> commands =
        [
            new MoveArmy
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveArmy
            {
                Issuer = Players.Player2,
                Origin = T(2),
                Path = [T(2), T(1)],
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Processed as a skirmish
        Assert.Multiple(() =>
        {
            commands.OfType<MoveArmy>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(4));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player2));
        });
    }


    [Test]
    public void ItResolvesLargerCircleAsSkirmish()
    {
        T(2).Owner = Players.Player3;
        T(2).Units.AddArmies(5);

        List<Command> commands =
        [
            new MoveArmy
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveArmy
            {
                Issuer = Players.Player3,
                Origin = T(2),
                Path = [T(2), T(3)],
            },
            new MoveArmy
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(3), T(1)],
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Processed as a skirmish
        Assert.Multiple(() =>
        {
            commands.OfType<MoveArmy>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(4));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player3));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player2));
        });
    }
}