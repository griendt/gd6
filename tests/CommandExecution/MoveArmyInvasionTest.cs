using engine;
using engine.Engine.Commands;
using engine.Engine.MoveResolutions;

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

}