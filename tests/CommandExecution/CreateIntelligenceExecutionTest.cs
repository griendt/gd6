using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateIntelligenceExecutionTest : BaseTest
{
    [SetUp]
    public void GrantIp()
    {
        Players.Player1.InfluencePoints = 300;
    }

    [Test]
    public void ItBuildsIntelligence()
    {
        var command = new CreateIntelligence { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Constructs, Does.Contain(Construct.Intelligence));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(280));
        });
    }
}