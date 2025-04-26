using engine.Engine.Commands;
using engine.Models;

namespace tests.CommandExecution;

public class CreateBivouacExecutionTest : BaseTest
{
    [SetUp]
    public void GrantIp()
    {
        Players.Player1.InfluencePoints = 300;
    }
    
    [Test]
    public void ItBuildsABivouac()
    {
        var command = new CreateBivouac { Issuer = Players.Player1, Origin = T(1) };

        command.Process(World);

        Assert.Multiple(() =>
        {
            Assert.That(T(1).Constructs, Does.Contain(Construct.Bivouac));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(270));
        });
    }

    [TestCase(0, 30)]
    [TestCase(1, 40)]
    [TestCase(2, 50)]
    [TestCase(10, 130)]
    public void ItIncreasesCostGivenExistingBivouacs(int numExistingBivouacs, int expectedCost)
    {
        Enumerable.Range(2, numExistingBivouacs).ToList()
            .ForEach(i =>
            {
                T(i).Owner = Players.Player1;
                T(i).Constructs.Add(Construct.Bivouac);
            });
        
        var command = new CreateBivouac { Issuer = Players.Player1, Origin = T(1) };
        
        command.Process(World);
        
        Assert.Multiple(() =>
        {
            Assert.That(T(1).Constructs, Does.Contain(Construct.Bivouac));
            Assert.That(Players.Player1.InfluencePoints, Is.EqualTo(300 - expectedCost));
        });
    }
    
}