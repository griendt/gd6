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
            .Select(i => new MoveArmy { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
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

    [TestCase(1, 2, 0, false)]
    [TestCase(2, 2, 0, false)]
    [TestCase(3, 2, 0, false)]
    [TestCase(4, 2, 0, true)]
    [TestCase(5, 1, 0, true)]
    [TestCase(6, 0, 0, true)]
    [TestCase(7, 0, 1, true)]
    [TestCase(10, 0, 4, true)]
    public void ItAppliesWatchtowerRulesForInvasion(int numSent, int numExpectedInTarget, int numExpectedUnprocessed, bool isWatchtowerRuined)
    {
        T(2).Constructs.Add(Construct.Watchtower);

        var invasions = Enumerable.Range(1, numSent)
            .Select(i => new MoveArmy { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
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
            Assert.That(T(2).Constructs, Does.Contain(isWatchtowerRuined ? Construct.Ruin : Construct.Watchtower));
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
            .Select(i => new MoveArmy { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
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
}