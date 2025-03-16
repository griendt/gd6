using System.ComponentModel.DataAnnotations;

namespace web.Models;

public class Player
{
    [Key] public Guid Id { get; init; }
    public required string Name { get; init; }

    [RegularExpression(@"^#[0-9a-f]{3,6}$")]
    public required string Colour { get; init; }
}