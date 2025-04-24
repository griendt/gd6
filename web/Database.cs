using System.ComponentModel.DataAnnotations;
using System.Text;
using engine;
using engine.Models;
using Microsoft.EntityFrameworkCore;
using web.Models;
using Player=web.Models.Player;
using Territory=web.Models.Territory;

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
            .UseSqlite($"Data Source={DbPath}");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Ensure coordinates are stored as JSON
        modelBuilder.Entity<Territory>().OwnsMany(
        navigationExpression: territory => territory.Coordinates,
        buildAction: ownedNavigationBuilder => ownedNavigationBuilder.ToJson());

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
            true);
        }

        if (errors.Count == 0) {
            return base.SaveChanges();
        }

        var errorMessages = new StringBuilder();
        errors.Each(error => errorMessages.AppendLine(error.ErrorMessage));

        throw new ValidationException(errorMessages.ToString());
    }

    public void FromWorld(World world)
    {
        var game = new Game { Name = "Global Domination VI", Id = Guid.NewGuid() };
        var players = world.Players
            .Select(player => new Player { Id = Guid.NewGuid(), Name = player.Name, Colour = "#fff" })
            .ToList();

        Games.Add(game);
        players.ForEach(player => GamePlayers.Add(new GamePlayer { Game = game, Player = player }));

        SaveChanges();
    }
}