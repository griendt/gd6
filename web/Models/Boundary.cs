using System.ComponentModel.DataAnnotations;

namespace web.Models;

public class Boundary
{
    [Key] public int Id { get; init; }

    public required int FromTerritoryId { get; init; }
    public virtual Territory FromTerritory { get; set; }

    public required int ToTerritoryId { get; init; }
    public virtual Territory ToTerritory { get; set; }
}