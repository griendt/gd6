using gdvi.Engine.Commands;

namespace tests.CommandExecution;

public class CreateHqExecutionTest : BaseTest
{
    [Test]
    public void ItBuildsAHq()
    {
        var command = new CreateHq { Issuer = Players.Player1, Origin = World.Territories[1] };
        
        command.Process(World);
        
        Assert.Multiple(() =>
        {
            Assert.That(World.Territories[1].HqSettler, Is.EqualTo(Players.Player1));
            Assert.That(World.Territories[1].Units.IsEmpty, Is.True);
        });
    }
}