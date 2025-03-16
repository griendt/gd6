using System.ComponentModel.DataAnnotations;

namespace web.Models;

public class Game
{
    [Key] public Guid Id { get; init; }
    public required string Name { get; init; }

    public virtual List<GamePlayer> GamePlayers { get; set; }
}