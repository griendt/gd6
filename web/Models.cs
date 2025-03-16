using System.ComponentModel.DataAnnotations;
using System.Text;
using engine;
using Microsoft.EntityFrameworkCore;

namespace web;

public class Gd6DbContext : DbContext
{
    public DbSet<Game> Games { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<GamePlayer> GamePlayers { get; set; }
    public DbSet<Territory> Territories { get; set; }
    public DbSet<Turn> Turns { get; set; }
    public DbSet<TerritoryTurn> TerritoryTurns { get; set; }
    public DbSet<HeadQuarter> HeadQuarters { get; set; }

    private static string DbPath => "/home/alex/projects/gd6/gd6.db";

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options
            .UseSqlite($"Data Source={DbPath}")
            .UseSeeding(new DataSeeder(this).Seed);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure coordinates are stored as JSON
        modelBuilder.Entity<Territory>().OwnsMany(
            territory => territory.Coordinates,
            ownedNavigationBuilder => ownedNavigationBuilder.ToJson());
    }

    public override int SaveChanges()
    {
        var changedEntities = ChangeTracker
            .Entries()
            .Where(entry => entry.State is EntityState.Added or EntityState.Modified);

        var errors = new List<ValidationResult>();

        foreach (var entry in changedEntities) {
            var validationContext = new ValidationContext(entry.Entity, null, null);
            Validator.TryValidateObject(
                entry.Entity,
                validationContext,
                errors,
                validateAllProperties: true);
        }

        if (errors.Count == 0) {
            return base.SaveChanges();
        }

        var errorMessages = new StringBuilder();
        errors.Each(error => errorMessages.AppendLine(error.ErrorMessage));

        throw new ValidationException(errorMessages.ToString());
    }
}

public class Territory
{
    [Key] public int Id { get; init; }
    public required string Identifier { get; set; }

    public required Game Game { get; init; }

    [MinLength(3)] public List<Coordinate> Coordinates { get; init; } = [];
    public List<TerritoryTurn> TerritoryTurns { get; init; }

    public Player? CurrentOwner()
    {
        if (TerritoryTurns == null || TerritoryTurns.Count == 0) {
            return null;
        }

        return TerritoryTurns
            .OrderBy(territoryTurn => territoryTurn.Turn.Id)
            .Reverse()
            .FirstOrDefault()
            ?.Owner;
    }

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

public class HeadQuarter
{
    [Key] public int Id { get; init; }
    public required string Name { get; set; }

    public Player Settler { get; init; }
}

public class Player
{
    [Key] public int Id { get; init; }
    public required string Name { get; init; }

    [RegularExpression(@"^#[0-9a-f]{3,6}$")]
    public required string Colour { get; init; }
}

public class Game
{
    [Key] public Guid Id { get; init; }
    public required string Name { get; init; }
}

public class GamePlayer
{
    [Key] public Guid Id { get; init; }
    public required Game Game { get; init; }
    public required Player Player { get; init; }
}

public class Turn
{
    [Key] public Guid Id { get; init; }
    public int TurnNumber { get; init; }

    public required Game Game { get; init; }
}

public class TerritoryTurn
{
    [Key] public int Id { get; init; }

    public required Turn Turn { get; init; }
    public required Territory Territory { get; init; }

    public HeadQuarter? HeadQuarter { get; init; }
    public Player? Owner { get; init; }
}