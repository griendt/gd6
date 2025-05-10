using engine;
using engine.Engine.Commands;
using engine.Engine.MoveResolutions;
using engine.Models;

namespace tests.CommandExecution;

public class MoveArmyInvasionTest : BaseTest
{
    [SetUp]
    public void SetUpForInvasion()
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(10);

        T(2).Owner = Players.Player2;
        T(2).Units.AddArmies(2);
    }

    [TestCase(1, 2, 0)]
    [TestCase(2, 2, 0)]
    [TestCase(3, 1, 0)]
    [TestCase(4, 0, 0)]
    [TestCase(5, 0, 1)]
    [TestCase(10, 0, 6)]
    public void ItAppliesBasicPenaltyForInvasion(int numSent, int numExpectedInTarget, int numExpectedUnprocessed)
    {
        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions
            .Take(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(invasion.IsProcessed));

        invasions
            .Skip(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(!invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(10 - numSent + numExpectedUnprocessed));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(T(2).Units.Armies, Is.EqualTo(numExpectedInTarget));
            Assert.That(T(2).IsNeutral, Is.EqualTo(numExpectedInTarget == 0));
        });
    }

    [TestCase(1, 1, 1, 1, 1, Description = "Invasion penalty goes first; defender suffers no losses")]
    [TestCase(2, 1, 1, 1, 1, Description = "Invasion penalty goes first; defender suffers no losses")]
    [TestCase(3, 1, 1, 0, 1, Description = "After invasion penalty, only the mine is destroyed; defending unit remains alive")]
    [TestCase(4, 1, 1, 0, 0, Description = "All units are neutralized")]
    [TestCase(8, 3, 8, 2, 3)]
    public void ItTriggersMinesInInvasion(int numSent, int numDefenders, int numMines, int numExpectedMines, int numDefendersLeft)
    {
        T(1).Units.AddArmies(numSent);
        T(2).Mines = numMines;
        T(2).Units.AddArmies(numDefenders - 2);// Undo the +2 from the setup

        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions.Each(invasion => Assert.That(invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(10));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(T(2).Units.Armies, Is.EqualTo(numDefendersLeft));
            Assert.That(T(2).IsNeutral, Is.EqualTo(numDefendersLeft == 0));
            Assert.That(T(2).Mines, Is.EqualTo(numExpectedMines));
        });
    }

    [TestCase(1, 2, 0, false)]
    [TestCase(2, 2, 0, false)]
    [TestCase(3, 2, 0, false)]
    [TestCase(4, 2, 0, true)]
    [TestCase(5, 1, 0, true)]
    [TestCase(6, 0, 0, true)]
    [TestCase(7, 0, 1, true)]
    [TestCase(10, 0, 4, true)]
    public void ItAppliesWatchtowerRulesForInvasion(int numSent, int numExpectedInTarget, int numExpectedUnprocessed, bool isWatchtowerDestroyed)
    {
        T(2).Constructs.Add(Construct.Watchtower);

        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions
            .Take(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(invasion.IsProcessed));

        invasions
            .Skip(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(!invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(10 - numSent + numExpectedUnprocessed));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(T(2).Units.Armies, Is.EqualTo(numExpectedInTarget));
            Assert.That(T(2).IsNeutral, Is.EqualTo(numExpectedInTarget == 0));
            Assert.That(T(2).Constructs, isWatchtowerDestroyed ? Does.Not.Contain(Construct.Watchtower) : Does.Contain(Construct.Watchtower));
        });
    }

    [TestCase(3, 1, 3, 0)]
    [TestCase(3, 2, 2, 0)]
    [TestCase(3, 3, 1, 0)]
    [TestCase(3, 4, 0, 0)]
    [TestCase(3, 5, 0, 1)]
    [TestCase(2, 1, 3, 0)]
    [TestCase(2, 2, 2, 0)]
    [TestCase(2, 3, 1, 0)]
    [TestCase(2, 4, 0, 0)]
    [TestCase(2, 5, 0, 1)]
    [TestCase(1, 1, 4, 0)]
    [TestCase(1, 5, 1, 0)]
    public void ItAppliesIntelligenceBonusToInvasion(int territoryIdWithIntelligence, int numSent, int numExpectedInTarget, int numExpectedUnprocessed)
    {
        T(1).Owner = Players.Player2;
        T(3).Owner = Players.Player2;
        T(3).Units.AddArmies(10);
        T(4).Owner = Players.Player3;
        T(4).Units.AddArmies(4);
        T(territoryIdWithIntelligence).Units.Add(Unit.Spy);

        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveUnit { Issuer = Players.Player2, Origin = T(3), Path = [T(3), T(4)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions
            .Take(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(invasion.IsProcessed));

        invasions
            .Skip(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(!invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(3).Units.Armies, Is.EqualTo(10 - numSent + numExpectedUnprocessed));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player2));
        });

        Assert.Multiple(() =>
        {
            Assert.That(T(4).Units.Armies, Is.EqualTo(numExpectedInTarget));
            Assert.That(T(4).IsNeutral, Is.EqualTo(numExpectedInTarget == 0));
        });
    }

    [Test]
    public void ItDoesNotApplyIntelligenceBonusIfNotOwned()
    {
        T(1).Units.Add(Unit.Spy);
        T(3).Owner = Players.Player2;
        T(3).Units.AddArmies(10);
        T(4).Owner = Players.Player3;
        T(4).Units.AddArmies(4);

        var invasions = Enumerable.Range(1, 4)
            .Select(i => new MoveUnit { Issuer = Players.Player2, Origin = T(3), Path = [T(3), T(4)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions.Each(invasion => Assert.That(invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(3).Units.Armies, Is.EqualTo(6));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player2));
            Assert.That(T(4).Units.Armies, Is.EqualTo(2));
        });
    }

    [TestCase(1, 2, 0)]
    [TestCase(2, 2, 0)]
    [TestCase(3, 1, 0)]
    [TestCase(4, 0, 0)]
    [TestCase(5, 0, 1)]
    [TestCase(10, 0, 6)]
    public void ItDestroysABivouac(int numSent, int numExpectedInTarget, int numExpectedUnprocessed)
    {
        T(2).Constructs.Add(Construct.Bivouac);

        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions
            .Take(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(invasion.IsProcessed));

        invasions
            .Skip(numSent - numExpectedUnprocessed)
            .Each(invasion => Assert.That(!invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(10 - numSent + numExpectedUnprocessed));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(T(2).Units.Armies, Is.EqualTo(numExpectedInTarget));
            Assert.That(T(2).IsNeutral, Is.EqualTo(numExpectedInTarget == 0));

            Assert.That(T(2).Constructs,
            numExpectedInTarget == 0
                ? Does.Not.Contain(Construct.Bivouac)
                : Does.Contain(Construct.Bivouac)
            );
        });
    }

    [TestCase(3, 1, 2)]
    [TestCase(4, 0, 2)]
    [TestCase(5, 0, 1)]
    [TestCase(6, 0, 0)]
    public void ItKillsArmiesBeforeCavalries(int numSent, int numArmiesExpectedInTarget, int numCavalryExpectedInTarget)
    {
        T(2).Units.AddCavalries(2);

        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Invasion().Resolve(invasions, World);

        invasions.Each(invasion => Assert.That(invasion.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(10 - numSent));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
        });

        Assert.Multiple(() =>
        {
            Assert.That(T(2).Units.Armies, Is.EqualTo(numArmiesExpectedInTarget));
            Assert.That(T(2).Units.Cavalries, Is.EqualTo(numCavalryExpectedInTarget));
            Assert.That(T(2).IsNeutral, Is.EqualTo(numArmiesExpectedInTarget == 0 && numCavalryExpectedInTarget == 0));
        });
    }
}