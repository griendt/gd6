using engine;
using engine.Engine;
using engine.Engine.Commands;
using engine.Models;

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

        World.AddBorder(T(1), T(3));
    }

    [Test]
    public void ItDetectsASimpleSkirmish()
    {
        List<Command> commands =
        [
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveUnit
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
            commands.OfType<MoveUnit>().Each(command => Assert.That(command.IsProcessed));

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
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveUnit
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
            commands.OfType<MoveUnit>().Each(command => Assert.That(command.IsProcessed));

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
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveUnit
            {
                Issuer = Players.Player3,
                Origin = T(2),
                Path = [T(2), T(3)],
            },
            new MoveUnit
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
            commands.OfType<MoveUnit>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(4));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player3));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player2));
        });
    }
    
    [Test]
    public void ItKillsSpiesAtNoCostInInvasion()
    {
        T(3).Units.Add(Unit.Spy, 37);

        var commands = Enumerable.Range(1, 5).Select(_ =>
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(3)],
            }
        ).OfType<Command>().ToList();

        new Turn { World = World, Commands = commands }.Process();

        // All spies are killed as well as 3 armies
        Assert.Multiple(() =>
        {
            commands.OfType<MoveUnit>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(0));
            Assert.That(T(3).Units.Armies, Is.EqualTo(2));
            Assert.That(T(3).Units.Spies, Is.EqualTo(0));
        });
    }
    
    [Test]
    public void SpiesTriggerSkirmishWithNoBenefit()
    {
        T(1).Units.Add(Unit.Spy);
        
        List<Command> commands =
        [
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
                UnitType = Unit.Spy,
            },
            new MoveUnit
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(3), T(2)],
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Processed as a skirmish. Spy is killed without dealing damage.
        // Thn player 2 occupies the neutral territory.
        Assert.Multiple(() =>
        {
            commands.OfType<MoveUnit>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(5));
            Assert.That(T(1).Units.Spies, Is.EqualTo(0));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));

            Assert.That(T(2).Units.Armies, Is.EqualTo(1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player2));
        });
    }
    
    [Test]
    public void SpiesAreUselessInSkirmish()
    {
        T(1).Units.Add(Unit.Spy);
        
        List<Command> commands =
        [
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
                UnitType = Unit.Spy,
            },
            new MoveUnit
            {
                Issuer = Players.Player1,
                Origin = T(1),
                Path = [T(1), T(2)],
            },
            new MoveUnit
            {
                Issuer = Players.Player2,
                Origin = T(3),
                Path = [T(3), T(2)],
            },
        ];

        new Turn { World = World, Commands = commands }.Process();

        // Processed as a skirmish. Spy is killed without dealing damage.
        Assert.Multiple(() =>
        {
            commands.OfType<MoveUnit>().Each(command => Assert.That(command.IsProcessed));

            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(1).Units.Spies, Is.EqualTo(0));
            Assert.That(T(3).Units.Armies, Is.EqualTo(4));

            Assert.That(T(2).Units.Armies, Is.EqualTo(0));
            Assert.That(T(2).IsNeutral);
        });
    }

}