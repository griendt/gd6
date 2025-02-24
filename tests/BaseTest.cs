using gdvi.Models;

namespace tests;

public abstract class BaseTest
{
    protected (Player Player1, Player Player2, Player Player3, Player Player4, Player Player5) Players;
    protected World World;

    [SetUp]
    public void SetupWorld()
    {
        World = new World();

        foreach (var id in Enumerable.Range(1, 20)) {
            World.Territories.Add(id, new Territory(World) { Id = id });

            if (id is <= 1 or > 20) {
                continue;
            }

            World.AddBorder(World.Territories[id - 1], World.Territories[id]);
        }
    }

    [SetUp]
    public void SetupPlayers()
    {
        Players = (
            new Player { Id = 1 },
            new Player { Id = 2 },
            new Player { Id = 3 },
            new Player { Id = 4 },
            new Player { Id = 5 }
        );
    }
}