using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateFortressExecutionTest : BaseTest
{
    [Test]
    public void ItBuildsAHq()
    {
        var command = new CreateFortress { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.That(T(1).Constructs, Does.Contain(Construct.Fortress));
    }
}