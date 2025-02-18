using gdvi.Models;

namespace tests;

public abstract class BaseTest
{
    protected World World;

    [SetUp]
    public void Setup()
    {
        World = new World();

        foreach (var id in Enumerable.Range(1, 5)) {
            World.Territories.Add(id, new Territory(World) { Id = id });

            if (id is <= 1 or > 5) {
                continue;
            }

            World.AddBorder(World.Territories[id - 1], World.Territories[id]);
        }
    }
}