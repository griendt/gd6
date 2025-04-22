using System.ComponentModel.DataAnnotations;
using engine.Models;

namespace web.Models;

public class Game
{
    [Key] public Guid Id { get; init; }
    public required string Name { get; init; }

    public virtual List<GamePlayer> GamePlayers { get; set; }
    public virtual List<Territory> Territories { get; set; }
    
    public World ToEngineWorld()
    {
        var world = new World();
    
        GamePlayers.ForEach(record => world.Players.Add(new engine.Models.Player
        {
            Id = record.Player.Id,
            Name = record.Player.Name,
        }));
        
        Territories.ForEach(record => world.Territories.Add(record.Id, new engine.Models.Territory(world)
        {
            Id = record.Id,
        }));

        return world;
    }
}