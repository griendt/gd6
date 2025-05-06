using engine.Engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.Phases;

public class ConstructionPhaseTest : BaseTest
{
    [SetUp]
    public void SetUp()
    {
        T(1).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 40;
    }

    [Test]
    public void ItRejectsPromotingAnArmyThatWasSpawnedInSameTurn()
    {
        var spawn = new SpawnArmy
        {
            Issuer = Players.Player1,
            Origin = T(1),
            Quantity = 1,
        };
        var promotion = new PromoteArmy
        {
            Issuer = Players.Player1,
            Origin = T(1),
            UnitType = Unit.Cavalry,
            Quantity = 1,
        };

        new Turn { World = World, Commands = [spawn, promotion] }.Process();

        // Army was spawned, but not promoted.
        Assert.Multiple(() =>
        {
            // No IP was deducted
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(41));
            Assert.That(T(1).Units.Armies, Is.EqualTo(1));
            Assert.That(T(1).Units.Cavalries, Is.EqualTo(0));

            // Insufficient armies at the time of promotion, since promotion comes before spawning armies.
            Assert.That(promotion.IsRejected);
            Assert.That(promotion.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientArmies));
        });
    }

    [Test]
    public void ItProcessesMultipleMinesInOneTerritory()
    {
        IEnumerable<Command> commands = Enumerable.Range(1, 5)
            .Select(_ => new CreateMine { Issuer = Players.Player1, Origin = T(1) });

        new Turn { Commands = commands.ToList(), World = World }.Process();

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Mines, Is.EqualTo(5));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(16));
        });
    }
}