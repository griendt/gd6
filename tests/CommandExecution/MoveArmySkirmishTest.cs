using engine;
using engine.Engine.Commands;
using engine.Engine.MoveResolutions;

namespace tests.CommandExecution;

public class MoveArmySkirmishTest : BaseTest
{
    [SetUp]
    public void SetUpForSkirmish()
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(5);

        T(3).Owner = Players.Player2;
        T(3).Units.AddArmies(5);
    }

    [Test]
    public void ItProcessesASimpleSkirmish()
    {
        List<MoveArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(2)],
            },
            new()
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(2)],
            },
        ];

        new Skirmish().Resolve(commands, World);

        // Both moves are considered processed
        commands.Each(command => Assert.That(command.IsProcessed));


        Assert.Multiple(() =>
        {
            // Both territories lost a unit
            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));

            // The middle territory remains neutral
            Assert.That(T(2).IsNeutral);
            Assert.That(T(2).Units.Armies, Is.EqualTo(0));
        });
    }

    [Test]
    public void ItProcessesAMutualAttackSkirmish()
    {
        T(2).Owner = Players.Player3;
        T(2).Units.AddArmies(5);

        List<MoveArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(2)],
            },
            new()
            {
                Issuer = Players.Player3,
                Origin = T(2),
                Path = [T(1)],
            },
        ];

        new Skirmish().Resolve(commands, World);

        // Both moves are considered processed
        commands.Each(command => Assert.That(command.IsProcessed));

        Assert.Multiple(() =>
        {
            // Both territories lost a unit
            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(4));
        });
    }

    [Test]
    public void ItProcessesALargerCircularSkirmish()
    {
        T(2).Owner = Players.Player3;
        T(2).Units.AddArmies(5);

        List<MoveArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(2)],
            },
            new()
            {
                Issuer = Players.Player3,
                Origin = T(2),
                Path = [T(3)],
            },
            new()
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(1)],
            },
        ];

        new Skirmish().Resolve(commands, World);

        // All moves are considered processed
        commands.Each(command => Assert.That(command.IsProcessed));

        // Each territory lost one unit
        Assert.Multiple(() => { ((int[]) [1, 2, 3]).Each(id => Assert.That(World.Territories[id].Units.Armies, Is.EqualTo(4))); });
    }


    [Test]
    public void ItLeavesLeftOverMovesAfterSkirmish()
    {
        List<MoveArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new()
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2), T(4)],
            },
            new()
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(3), T(2)],
            },
        ];

        new Skirmish().Resolve(commands, World);

        // Two of three moves are considered processed
        Assert.That(commands.Count(command => command.IsProcessed), Is.EqualTo(2));

        // The unprocessed move is the second command by player 1.
        // This is because the first move is considered higher priority.
        var unprocessedMove = commands.First(command => !command.IsProcessed);
        Assert.That(unprocessedMove.Path, Has.Count.EqualTo(3));

        Assert.Multiple(() =>
        {
            // Both territories lost a unit
            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));

            // The middle territory remains neutral.
            // It will be overtaken only later by the last remaining move.
            Assert.That(T(2).IsNeutral);
            Assert.That(T(2).Units.Armies, Is.EqualTo(0));
        });
    }
}