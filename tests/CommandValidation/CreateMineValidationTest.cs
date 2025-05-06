using engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class CreateMineValidationTest : BaseTest
{
    [SetUp]
    public void SetUpInfluencePoints()
    {
        Enumerable.Range(1, 10).Each(i => T(i).Owner = Players.Player1);
        Players.Player1.InfluencePoints = 52;
    }

    [TestCase(1, false)]
    [TestCase(2, false)]
    [TestCase(10, false)]
    [TestCase(11, true)]
    public void ItChecksAvailableInfluencePoints(int numMines, bool shouldBeRejected)
    {
        var commands = Enumerable.Range(1, numMines)
            .Select(_ => new CreateMine { Issuer = Players.Player1, Origin = T(1) })
            .ToList();

        CommandValidator.Validate(commands, World);

        commands.ForEach(command =>
        {
            Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
            if (shouldBeRejected) {
                Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
            }
        });
    }

    [Test]
    public void ItChecksOwnershipForLayingMines()
    {
        var command = new CreateMine { Issuer = Players.Player1, Origin = T(11) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Test]
    public void ItAllowsForWasteland()
    {
        T(1).IsWasteland = true;
        var command = new CreateMine { Issuer = Players.Player1, Origin = T(1) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected, Is.False);
    }
}