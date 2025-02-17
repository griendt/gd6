using gdvi.Models;

namespace tests;

public class Tests
{
    private World _world;

    [SetUp]
    public void Setup()
    {
        _world = new World();

        foreach (var id in Enumerable.Range(1, 5)) {
            _world.Territories.Add(id, new Territory(_world) { Id = id });

            if (id is <= 1 or > 5) {
                continue;
            }

            _world.AddBorder(_world.Territories[id - 1], _world.Territories[id]);
        }
    }

    [Test]
    public void TestBoundaries()
    {
        foreach (var id in Enumerable.Range(2, 3))
        {
            Assert.Multiple(() =>
            {
                Assert.That(_world.Territories[id].IsNeighbour(_world.Territories[id + 1]));
                Assert.That(_world.Territories[id - 1].IsNeighbour(_world.Territories[id]));
            });
        }
    }
}