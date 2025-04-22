using System.ComponentModel.DataAnnotations;

namespace web.Models;


public class Territory
{
    [Key] public int Id { get; init; }
    public required string Identifier { get; init; }

    public Guid GameId { get; init; }
    public virtual Game Game { get; set; }

    [MinLength(3)] public List<Coordinate> Coordinates { get; init; }

    public virtual List<TerritoryTurn> TerritoryTurns { get; set; }

    public Player? CurrentOwner() =>
        TerritoryTurns
            .OrderBy(territoryTurn => territoryTurn.Turn.Id)
            .Reverse()
            .FirstOrDefault()
            ?.Owner;

    public (int X, int Y) Centroid
    {
        get
        {
            var centroidX = Enumerable
                .Range(0, Coordinates.Count)
                .Select(i =>
                    (Coordinates[i].X + Coordinates[(i + 1) % Coordinates.Count].X) *
                    (Coordinates[i].X * Coordinates[(i + 1) % Coordinates.Count].Y - Coordinates[(i + 1) % Coordinates.Count].X * Coordinates[i].Y))
                .Sum() / (6 * SignedArea);

            var centroidY = Enumerable
                .Range(0, Coordinates.Count)
                .Select(i =>
                    (Coordinates[i].Y + Coordinates[(i + 1) % Coordinates.Count].Y) *
                    (Coordinates[i].X * Coordinates[(i + 1) % Coordinates.Count].Y - Coordinates[(i + 1) % Coordinates.Count].X * Coordinates[i].Y))
                .Sum() / (6 * SignedArea);

            return (centroidX, centroidY);
        }
    }

    public int SignedArea => Enumerable
        .Range(0, Coordinates.Count)
        .Select(i => Coordinates[i].X * Coordinates[(i + 1) % Coordinates.Count].Y - Coordinates[(i + 1) % Coordinates.Count].X * Coordinates[i].Y)
        .Sum() / 2;
}

public class Coordinate
{
    public int X { get; init; }
    public int Y { get; init; }
}
