using System.ComponentModel.DataAnnotations;

namespace web.Models;


public class TerritoryTurn
{
    [Key] public Guid Id { get; init; }

    public Guid TurnId { get; init; }
    public virtual Turn Turn { get; set; }
    
    public Guid TerritoryId { get; init; }
    public virtual Territory Territory { get; set; }

    public Guid? HeadQuarterId { get; init; }
    public virtual HeadQuarter? HeadQuarter { get; set; }
    
    public Guid? OwnerId { get; init; }
    public virtual Player? Owner { get; set; }
}