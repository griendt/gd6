using engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class CreateLibraryValidationTest : BaseTest
{
    [SetUp]
    public void SetUpInfluencePoints()
    {
        Enumerable.Range(1, 10).Each(i => T(i).Owner = Players.Player1);
        Players.Player1.InfluencePoints = 50;
    }

    [TestCase(0, 1, false)]
    [TestCase(0, 5, false)]
    [TestCase(0, 6, true)]
    [TestCase(1, 2, false)]
    [TestCase(1, 3, true)]
    [TestCase(2, 1, false)]
    [TestCase(2, 2, true)]
    [TestCase(4, 1, false)]
    [TestCase(4, 2, true)]
    [TestCase(5, 1, true)]
    public void ItChecksAvailableInfluencePoints(int numExisting, int numNew, bool shouldBeRejected)
    {
        Enumerable.Range(1, numExisting)
            .Each(i =>
            {
                T(i).Owner = Players.Player1;
                T(i).Constructs.Add(Construct.Library);
            }); 
        
        var commands = Enumerable.Range(numExisting + 1, numNew)
            .Select(i => new CreateLibrary { Issuer = Players.Player1, Origin = T(i) })
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
    public void ItChecksBuildingMultipleLibrariesInOneTerritory()
    {
        var commands = Enumerable.Range(1, 2)
            .Select(i => new CreateLibrary { Issuer = Players.Player1, Origin = T(1) })
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
    public void ItChecksOwnershipForBuildingLibrary()
    {
        var command = new CreateLibrary { Issuer = Players.Player1, Origin = T(11) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Test]
    public void ItChecksForWasteland()
    {
        T(1).IsWasteland = true;
        var command = new CreateLibrary { Issuer = Players.Player1, Origin = T(1) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.BuildingOnToxicWasteland));
    }
}