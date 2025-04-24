using System.ComponentModel.DataAnnotations;

namespace web.Models;


public class TerritoryBorder
{
    [Key] public int Id { get; init; }

    public Guid GameId { get; init; }
    public virtual Game Game { get; set; }

    public int FirstTerritoryId { get; init; }
    public virtual Territory FirstTerritory { get; set; }
    
    public int SecondTerritoryId { get; init; }
    public virtual Territory SecondTerritory { get; set; }
}