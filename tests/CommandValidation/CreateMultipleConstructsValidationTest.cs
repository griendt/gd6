using engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandValidation;

public class CreateMultipleConstructsValidationTest : BaseTest
{
    [SetUp]
    public void SetUpInfluencePoints()
    {
        T(1).Owner = Players.Player1;
        T(2).Owner = Players.Player1;
        T(3).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 60;
    }

    [TestCase(40, true)]
    [TestCase(50, true)]
    [TestCase(60, false)]
    public void ItChecksForSufficientInfluencePoints(int influencePoints, bool shouldBeRejected)
    {
        T(3).Constructs.Add(Construct.Bivouac);

        Players.Player1.InfluencePoints = influencePoints;
        List<Command> commands =
        [
            new CreateBivouac { Issuer = Players.Player1, Origin = T(1) },
            new CreateWatchtower { Issuer = Players.Player1, Origin = T(2) },
        ];

        CommandValidator.Validate(commands, World);

        commands.Each(command =>
        {
            Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
            if (shouldBeRejected) {
                Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
            }
        });
    }
    
    [TestCase(20, true)]
    [TestCase(29, true)]
    [TestCase(30, false)]
    public void ItChecksForSufficientInfluencePointsWhenBuildingMines(int influencePoints, bool shouldBeRejected)
    {
        Players.Player1.InfluencePoints = influencePoints;
        List<Command> commands =
        [
            new CreateWatchtower { Issuer = Players.Player1, Origin = T(1) },
            new CreateMine { Issuer = Players.Player1, Origin = T(1) },
            new CreateMine { Issuer = Players.Player1, Origin = T(1) },
        ];

        CommandValidator.Validate(commands, World);

        commands.Each(command =>
        {
            Assert.That(command.IsRejected, Is.EqualTo(shouldBeRejected));
            if (shouldBeRejected) {
                Assert.That(command.Rejections.First().Reason, Is.EqualTo(RejectReason.InsufficientInfluencePoints));
            }
        });
    }
}