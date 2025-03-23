using engine.Engine.Commands;

namespace tests.CommandExecution;

public class MoveArmySkirmishTest : BaseTest
{
    [SetUp]
    public void SetUpForSkirmish()
    {
        World.Territories[1].Owner = Players.Player1;
        World.Territories[1].Units.AddArmies(5);

        World.Territories[3].Owner = Players.Player2;
        World.Territories[3].Units.AddArmies(5);
    }

    [Test]
    public void ItProcessesASimpleSkirmish()
    {
        List<MoveArmy> commands =
        [
            new()
            {
                Issuer = Players.Player1,
                Origin = World.Territories[1],
                Path = [World.Territories[2]],
            },
            new()
            {
                Issuer = Players.Player2,
                Origin = World.Territories[3],
                Path = [World.Territories[2]],
            },
        ];

        var resultingMoves = MoveArmy.ProcessSkirmish(commands);

        Assert.Multiple(() =>
        {
            // Moves are resolved without any resulting moves left to process
            Assert.That(resultingMoves, Is.Empty);

            // Both territories lost a unit
            Assert.That(World.Territories[1].Units.Armies, Is.EqualTo(4));
            Assert.That(World.Territories[3].Units.Armies, Is.EqualTo(4));

            // The middle territory remains neutral
            Assert.That(World.Territories[2].IsNeutral);
            Assert.That(World.Territories[2].Units.Armies, Is.EqualTo(0));
        });
    }
}