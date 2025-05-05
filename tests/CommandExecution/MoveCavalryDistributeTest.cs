using engine.Engine.Commands;
using engine.Engine.MoveResolutions;

namespace tests.CommandExecution;

public class MoveCavalryDistributeTest : BaseTest
{
    [SetUp]
    public void SetUpForDistribute()
    {
        T(1).Owner = Players.Player1;
        T(1).Units.AddCavalries(5);
    }

    [Test]
    public void ItDistributesACavalry()
    {
        var move = new MoveCavalry
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Path = [T(1), T(2)],
        };

        new Distribute().Resolve([move], World);

        Assert.That(move.IsProcessed);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Cavalries, Is.EqualTo(4));
            Assert.That(T(2).Units.Cavalries, Is.EqualTo(1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
        });
    }

    [Test]
    public void ItDistributesACavalryInALongerPathInSteps()
    {
        var move = new MoveCavalry
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Path = [T(1), T(2), T(3), T(4), T(5)],
        };

        new Distribute().Resolve([move], World);

        Assert.Multiple(() =>
        {
            // The move is not yet fully resolved; only one step has been done.
            Assert.That(!move.IsProcessed);
            Assert.That(T(1).Units.Cavalries, Is.EqualTo(4));
            Assert.That(T(2).Units.Cavalries, Is.EqualTo(1));
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
        });

        new Distribute().Resolve([move], World);
        
        Assert.Multiple(() =>
        {
            // The move is still not yet fully resolved
            Assert.That(!move.IsProcessed);
            Assert.That(T(1).Units.Cavalries, Is.EqualTo(4));
            Assert.That(T(2).Units.Cavalries, Is.EqualTo(0));
            Assert.That(T(3).Units.Cavalries, Is.EqualTo(1));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player1));
        });
        
        new Distribute().Resolve([move], World);
        new Distribute().Resolve([move], World);

        Assert.Multiple(() =>
        {
            // Now the move is fully resolved. Effectively, one cavalry has moved
            // from `1` to `5`.
            Assert.That(move.IsProcessed);
            Assert.That(T(1).Units.Cavalries, Is.EqualTo(4));
            Assert.That(T(2).Units.Cavalries, Is.EqualTo(0));
            Assert.That(T(3).Units.Cavalries, Is.EqualTo(0));
            Assert.That(T(4).Units.Cavalries, Is.EqualTo(0));
            Assert.That(T(5).Units.Cavalries, Is.EqualTo(1));
            
            // Even though territories 2-4 have no unit on it at the end of the resolution,
            // it will belong to player 1 because he passed his unit through it.
            Assert.That(T(2).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(3).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(4).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(5).Owner, Is.EqualTo(Players.Player1));
        });
    }
}