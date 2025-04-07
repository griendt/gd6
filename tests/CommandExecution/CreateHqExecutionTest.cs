using engine.Engine.Commands;

namespace tests.CommandExecution;

public class CreateHqExecutionTest : BaseTest
{
    [Test]
    public void ItBuildsAHq()
    {
        var command = new CreateHq { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).HqSettler, Is.EqualTo(Players.Player1));
            Assert.That(T(1).Owner, Is.EqualTo(Players.Player1));
            Assert.That(T(1).Units.IsEmpty, Is.True);
        });
    }
}