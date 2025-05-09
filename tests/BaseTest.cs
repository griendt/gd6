using engine.Models;

namespace tests;

public abstract class BaseTest
{
    protected (Player Player1, Player Player2, Player Player3, Player Player4, Player Player5) Players;
    protected World World;

    protected Territory T(int id)
    {
        return World.Territories[id];
    }

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
            new Player { Id = Guid.NewGuid(), Name = "A" },
            new Player { Id = Guid.NewGuid(), Name = "B" },
            new Player { Id = Guid.NewGuid(), Name = "C" },
            new Player { Id = Guid.NewGuid(), Name = "D" },
            new Player { Id = Guid.NewGuid(), Name = "E" }
        );

        World.Players =
        [
            Players.Player1,
            Players.Player2,
            Players.Player3,
            Players.Player4,
            Players.Player5,
        ];
    }
}