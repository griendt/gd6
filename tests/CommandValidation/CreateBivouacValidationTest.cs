using engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class CreateBivouacValidationTest : BaseTest
{
    [SetUp]
    public void SetUpInfluencePoints()
    {
        T(1).Owner = Players.Player1;
        T(2).Owner = Players.Player1;
        T(3).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 60;
    }

    [TestCase(1, false)]
    [TestCase(2, false)]
    [TestCase(3, true)]
    public void ItChecksAvailableInfluencePoints(int numBivouacs, bool shouldBeRejected)
    {
        // The cost of each bivouac increases per bivouac already on the map.
        // However, if multiple bivouacs are ordered simultaneously, only the base cost is applied!
        // According to the rules: the cost increases per bivouac owned *at the start of the Turn*!
        var commands = Enumerable.Range(1, numBivouacs)
            .Select(i => new CreateBivouac { Issuer = Players.Player1, Origin = T(i) })
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

    [TestCase(30, true)]
    [TestCase(40, true)]
    [TestCase(45, true)]
    [TestCase(55, false)]
    public void ItChecksExistingBivouacsForIpCost(int influencePoints, bool shouldBeRejected)
    {
        Players.Player1.InfluencePoints = influencePoints;
        T(1).Constructs.Add(Construct.Bivouac);
        T(2).Constructs.Add(Construct.Bivouac);

        var command = new CreateBivouac { Issuer = Players.Player1, Origin = T(3) };
        
        CommandValidator.Validate([command], World);
        
        Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
        if (shouldBeRejected) {
            Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
        }
    }

    [TestCase(30, 1, true)]
    [TestCase(35, 1, true)]
    [TestCase(40, 1, false)]
    [TestCase(75, 2, true)]
    [TestCase(80, 2, false)]
    [TestCase(120, 3, false)]
    public void ItIgnoresOtherConstructsForIpCost(int influencePoints, int numBivouacs, bool shouldBeRejected)
    {
        Players.Player1.InfluencePoints = influencePoints;
        T(1).Constructs.Add(Construct.Bivouac);
        T(2).Constructs.Add(Construct.Fortress);

        var commands = Enumerable.Range(1, numBivouacs)
            .Select(i =>
            {
                T(2 + i).Owner = Players.Player1;
                return new CreateBivouac { Issuer = Players.Player1, Origin = T(2 + i) };
            })
            .ToList();
        
        CommandValidator.Validate(commands, World);
        
        commands.Each(command =>
        {
            Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
            if (shouldBeRejected) {
                Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
            }
        });
    }

    [Test]
    public void ItChecksBuildingMultipleBivouacsInOneTerritory()
    {
        var commands = Enumerable.Range(1, 2)
            .Select(i => new CreateBivouac { Issuer = Players.Player1, Origin = T(1) })
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
    public void ItChecksOwnershipForBuildingBivouac()
    {
        var command = new CreateBivouac { Issuer = Players.Player1, Origin = T(4) };

        CommandValidator.Validate([command], World);

        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Test]
    public void ItChecksForWasteland()
    {
        T(1).IsWasteland = true;
        var command = new CreateBivouac { Issuer = Players.Player1, Origin = T(1) };
        
        CommandValidator.Validate([command], World);
        
        Assert.That(command.IsRejected);
        Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.BuildingOnToxicWasteland));
    }
}