using System.ComponentModel.DataAnnotations;

namespace web.Models;

public class HeadQuarter
{
    [Key] public Guid Id { get; init; }
    public required string Name { get; set; }

    public Guid SettlerId { get; init; }
    public virtual Player Settler { get; set; }
}
