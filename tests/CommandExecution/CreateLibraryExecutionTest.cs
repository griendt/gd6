using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateLibraryExecutionTest : BaseTest
{
    [SetUp]
    public void GrantIp()
    {
        Players.Player1.InfluencePoints = 300;
    }

    [Test]
    public void ItBuildsALibrary()
    {
        var command = new CreateLibrary { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Constructs, Does.Contain(Construct.Library));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(290));
        });
    }
    
    
    [TestCase(0, 10)]
    [TestCase(1, 20)]
    [TestCase(2, 30)]
    [TestCase(10, 110)]
    public void ItIncreasesCostGivenExistingLibraries(int numExisting, int expectedCost)
    {
        Enumerable.Range(2, numExisting).ToList()
            .ForEach(i =>
            {
                T(i).Owner = Players.Player1;
                T(i).Constructs.Add(Construct.Library);
            });
        
        var command = new CreateLibrary { Issuer = Players.Player1, Origin = T(1) };
        
        command.Process(World);
        
        Assert.Multiple(() =>
        {
            Assert.That(T(1).Constructs, Does.Contain(Construct.Library));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(300 - expectedCost));
        });
    }
}