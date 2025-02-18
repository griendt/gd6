namespace tests;

public class WorldSetupTest : BaseTest
{
    
    [Test]
    public void TestBoundaries()
    {
        foreach (var id in Enumerable.Range(2, 3))
        {
            Assert.Multiple(() =>
            {
                Assert.That(World.Territories[id].IsNeighbour(World.Territories[id + 1]));
                Assert.That(World.Territories[id - 1].IsNeighbour(World.Territories[id]));
            });
        }
    }

}