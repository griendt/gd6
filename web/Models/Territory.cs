using System.ComponentModel.DataAnnotations;

namespace web.Models;

public class Territory
{
    [Key] public int Id { get; init; }
    public required string Identifier { get; init; }

    [MinLength(3)] public List<Coordinate> Coordinates { get; init; }
    public virtual List<Boundary> Boundaries { get; init; }

    public Guid? PlayerId { get; init; }
    public virtual Player? Owner { get; set; }

    public Guid? HeadquarterId { get; init; }
    public virtual HeadQuarter? Headquarter { get; set; }

    public List<Construct> Constructs { get; set; } = [];

    public int Armies { get; set; }
    public int Cavalries { get; set; }
    public int Heavies { get; set; }

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

    public List<Territory> BoundariesOverWater() => Boundaries
        .Select(boundary => boundary.ToTerritory)
        .Where(otherTerritory => otherTerritory
            .Coordinates
            .Select(c => (c.X, c.Y))
            .Intersect(Coordinates.Select(c => (c.X, c.Y)))
            .ToList().Count == 0)
        .ToList();
}

public class Coordinate
{
    public int X { get; init; }
    public int Y { get; init; }
}

public class Construct
{
    public required string Name { get; init; }
}