using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateBivouacExecutionTest : BaseTest
{
    [Test]
    public void ItBuildsAHq()
    {
        var command = new CreateBivouac { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.That(T(1).Constructs, Does.Contain(Construct.Bivouac));
    }
}