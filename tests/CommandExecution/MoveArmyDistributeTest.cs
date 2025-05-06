using engine.Engine.Commands;
using engine.Engine.MoveResolutions;
using engine.Models;

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
        var move = new MoveUnit
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
        var move = new MoveUnit
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

    [TestCase(1, 1)]
    [TestCase(1, 3)]
    [TestCase(3, 3)]
    [TestCase(20, 6)]
    public void ItTriggersMines(int numMoves, int numMines)
    {
        T(2).Mines = numMines;
        T(1).Units.AddArmies(numMoves);

        var moves = Enumerable
            .Range(1, numMoves)
            .Select(_ => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Distribute().Resolve(moves, World);

        moves.ForEach(move => Assert.That(move.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Armies, Is.EqualTo(5));
            Assert.That(T(2).Units.Armies, Is.EqualTo(int.Max(numMoves - numMines, 0)));
            Assert.That(T(2).Owner, numMoves > numMines ? Is.EqualTo(Players.Player1) : Is.Null);
            Assert.That(T(2).Mines, Is.EqualTo(int.Max(numMines - numMoves, 0)));
        });
    }

    [Test]
    public void ItDoesNotTriggerMinesInDistributeToOwnedTerritory()
    {
        T(2).Mines = 3;
        T(2).Owner = Players.Player1;

        var moves = Enumerable
            .Range(1, 2)
            .Select(_ => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)] })
            .ToList();

        new Distribute().Resolve(moves, World);

        moves.ForEach(move => Assert.That(move.IsProcessed));

        Assert.Multiple(() =>
        {
            // Distribute as usual; mines are left untouched
            Assert.That(T(1).Units.Armies, Is.EqualTo(3));
            Assert.That(T(2).Units.Armies, Is.EqualTo(2));
            Assert.That(T(2).Mines, Is.EqualTo(3));
        });
    }

    [TestCase(1, 1, 1, 0)]
    [TestCase(1, 2, 0, 0)]
    [TestCase(1, 3, 0, 1)]
    [TestCase(3, 3, 2, 0)]
    [TestCase(3, 1, 3, 0)]
    [TestCase(3, 4, 1, 0)]
    [TestCase(3, 5, 1, 0)]
    [TestCase(8, 21, 0, 5)]
    [TestCase(23, 5, 21, 0)]
    public void MinesDealOneDamage(int numHeavies, int numMines, int numExpectedInTarget, int numExpectedRemainingMines)
    {
        T(2).Mines = numMines;
        T(1).Units.Add(Unit.Heavy, numHeavies);

        var moves = Enumerable
            .Range(1, numHeavies)
            .Select(_ => new MoveUnit { Issuer = Players.Player1, Origin = T(1), Path = [T(1), T(2)], UnitType = Unit.Heavy })
            .ToList();

        new Distribute().Resolve(moves, World);

        moves.ForEach(move => Assert.That(move.IsProcessed));

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Units.Heavies, Is.EqualTo(0));
            Assert.That(T(2).Units.Heavies, Is.EqualTo(numExpectedInTarget));
            Assert.That(T(2).Owner, numExpectedInTarget > 0 ? Is.EqualTo(Players.Player1) : Is.Null);
            Assert.That(T(2).Mines, Is.EqualTo(numExpectedRemainingMines));
        });
    }
}