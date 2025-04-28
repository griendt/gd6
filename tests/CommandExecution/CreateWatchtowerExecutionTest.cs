using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateWatchtowerExecutionTest : BaseTest
{
    [SetUp]
    public void GrantIp()
    {
        Players.Player1.InfluencePoints = 300;
    }

    [Test]
    public void ItBuildsAWatchtower()
    {
        var command = new CreateWatchtower { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Constructs, Does.Contain(Construct.Watchtower));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(280));
        });
    }
}