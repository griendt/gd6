using System.ComponentModel.DataAnnotations;

namespace web.Models;

public class GamePlayer
{
    [Key] public Guid Id { get; init; }

    public Guid GameId { get; init; }
    public virtual Game Game { get; set; }

    public Guid PlayerId { get; init; }
    public virtual Player Player { get; set; }
}