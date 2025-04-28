using engine.Engine.Commands;

namespace tests.CommandValidation;

public class CreateIntelligenceValidationTest : BaseTest
{
    [SetUp]
    public void SetUpInfluencePoints()
    {
        T(1).Owner = Players.Player1;
        T(2).Owner = Players.Player1;
        T(3).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 50;
    }

    [TestCase(1, false)]
    [TestCase(2, false)]
    [TestCase(3, true)]
    public void ItChecksAvailableInfluencePoints(int numIntelligence, bool shouldBeRejected)
    {
        var commands = Enumerable.Range(1, numIntelligence)
            .Select(i => new CreateIntelligence { Issuer = Players.Player1, Origin = T(i) })
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
    public void ItChecksBuildingMultipleIntelligenceInOneTerritory()
    {
        var commands = Enumerable.Range(1, 2)
            .Select(i => new CreateIntelligence { Issuer = Players.Player1, Origin = T(1) })
            .ToList();

        CommandValidator.Validate(commands, World);

        commands.ForEach(command =>
        {
            Assert.Multiple(() =>
            {
                Assert.That(command.IsRejected);
                Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.BuildingMultipleConstructsInOneTerritory));
            });
        });
    }

    [Test]
    public void ItChecksOwnershipForBuildingIntelligence()
    {
        var command = new CreateIntelligence { Issuer = Players.Player1, Origin = T(4) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Test]
    public void ItChecksForWasteland()
    {
        T(1).IsWasteland = true;
        var command = new CreateIntelligence { Issuer = Players.Player1, Origin = T(1) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.BuildingOnToxicWasteland));
    }
}