using engine.Engine;
using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateMineExecutionTest : BaseTest
{
    [SetUp]
    public void GrantIp()
    {
        T(1).Owner = Players.Player1;
        Players.Player1.InfluencePoints = 300;
    }

    [Test]
    public void ItCreatesAMine()
    {
        var command = new CreateMine { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Mines, Is.EqualTo(1));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(295));
        });
    }
    
}