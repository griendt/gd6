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
    public int Spies { get; set; }
    public int Mines { get; set; }
    public int Loyalty { get; set; }

    public (long X, long Y) Centroid
    {
        get
        {
            // Optimisation so that we can work with relatively small numbers.
            // This avoids overflow errors when working with many coordinates with large numbers in them.
            var offsetX = Coordinates.First().X;
            var offsetY = Coordinates.First().Y;
            
            var cs = Coordinates
                .Select(coordinate => new Coordinate { X = coordinate.X - offsetX, Y = coordinate.Y - offsetY })
                .ToList();
            
            var centroidX = Enumerable
                .Range(0, cs.Count)
                .Select(i =>
                    (cs[i].X + cs[(i + 1) % cs.Count].X) *
                    (cs[i].X * cs[(i + 1) % cs.Count].Y - cs[(i + 1) % cs.Count].X * cs[i].Y))
                .Sum() / (6 * SignedArea);

            var centroidY = Enumerable
                .Range(0, cs.Count)
                .Select(i =>
                    (cs[i].Y + cs[(i + 1) % cs.Count].Y) *
                    (cs[i].X * cs[(i + 1) % cs.Count].Y - cs[(i + 1) % cs.Count].X * cs[i].Y))
                .Sum() / (6 * SignedArea);

            return (centroidX + offsetX, centroidY + offsetY);
        }
    }

    public long SignedArea => Enumerable
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
    public long X { get; init; }
    public long Y { get; init; }
}

public class Construct
{
    public required string Name { get; init; }
}