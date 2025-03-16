using System.ComponentModel.DataAnnotations;

namespace web.Models;


public class Turn
{
    [Key] public Guid Id { get; init; }
    public int TurnNumber { get; init; }

    public Guid GameId { get; init; }
    public virtual Game Game { get; set; }

    public virtual List<TerritoryTurn> TerritoryTurns { get; set; } = [];
}
