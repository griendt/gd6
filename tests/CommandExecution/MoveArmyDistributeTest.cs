using engine.Engine.Commands;
using engine.Engine.MoveResolutions;

namespace tests.CommandExecution;

public class MoveArmyDistributeTest : BaseTest
{
    [SetUp]
    public void SetUpForDistribute()
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddArmies(5);
    }

    [Test]
    public void ItDistributesAnArmy()
    {
        var move = new MoveArmy
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Path = [T(1), T(2)],
        };

        new Distribute().Resolve([move], World);

        Assert.That(move.IsProcessed);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
        });
    }


    [Test]
    public void ItDistributesAnArmyInALongerPathInSteps()
    {
        var move = new MoveArmy
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Path = [T(1), T(2), T(3)],
        };

        new Distribute().Resolve([move], World);

        Assert.Multiple(() =>
        {
            // The move is not yet fully resolved; only one step has been done.
            Assert.That(!move.IsProcessed);
            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
        });

        new Distribute().Resolve([move], World);

        Assert.Multiple(() =>
        {
            // Now the move is fully resolved. Effectively, one army has moved
            // from `1` to `3`.
            Assert.That(move.IsProcessed);
            Assert.That(T(1).Units.Armies, Is.EqualTo(4));
            Assert.That(T(2).Units.Armies, Is.EqualTo(0));
            Assert.That(T(3).Units.Armies, Is.EqualTo(1));

            // Even though territory 2 has no unit on it at the end of the resolution,
            // it will belong to player 1 because he passed his unit through it.
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player1));
        });
    }
}