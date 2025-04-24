using System.ComponentModel.DataAnnotations;
using System.Text;
using engine;
using Microsoft.EntityFrameworkCore;
using web.Models;

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
            .UseLazyLoadingProxies()
            .UseSqlite($"Data Source={DbPath}")
            .UseSeeding(new DataSeeder(this).Seed);

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure coordinates are stored as JSON
        modelBuilder.Entity<Territory>().OwnsMany(
            territory => territory.Coordinates,
            ownedNavigationBuilder => ownedNavigationBuilder.ToJson());

        modelBuilder.Entity<Territory>()
            .HasMany(territory => territory.TerritoryBorders)
            .WithOne(border => border.FirstTerritory);
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
