using System.ComponentModel.DataAnnotations;
using System.Text;
using engine;
using engine.Models;
using Microsoft.EntityFrameworkCore;
using web.Models;
using Construct=web.Models.Construct;
using Player=web.Models.Player;
using Territory=web.Models.Territory;

namespace web;

public class Gd6DbContext : DbContext
{
    public DbSet<Player> Players { get; set; }
    public DbSet<Territory> Territories { get; set; }
    public DbSet<HeadQuarter> HeadQuarters { get; set; }
    public DbSet<Boundary> Boundaries { get; set; }

    private static string DbPath => "/home/alex/projects/gd6/files/gd6.db";

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

        // Ensure constructs are also stored as JSON
        modelBuilder.Entity<Territory>().OwnsMany(
        navigationExpression: territory => territory.Constructs,
        buildAction: ownedNavigationBuilder => ownedNavigationBuilder.ToJson());

        modelBuilder.Entity<Territory>()
            .HasMany(territory => territory.Boundaries)
            .WithOne(boundary => boundary.FromTerritory);

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
        world.Players
            .ForEach(player => Players.Add(new Player { Id = player.Id, Name = player.Name, Colour = player.Colour }));

        world.Territories
            .Values
            .ToList()
            .ForEach(territory => Territories.Add(new Territory
            {
                Id = territory.Id,
                Identifier = $"{territory.Id}",
                Coordinates = territory.Coordinates.Select(xy => new Coordinate { X = xy.Item1, Y = xy.Item2 }).ToList(),
                Headquarter = null,
            }));

        world.TerritoryBorders
            .ToList()
            .ForEach(borders =>
            {
                borders.Value.ForEach(neighbour =>
                {
                    // Symmetry is guaranteed by the symmetry of `world.TerritoryBorders`
                    Boundaries.Add(new Boundary
                    {
                        FromTerritoryId = borders.Key,
                        ToTerritoryId = neighbour,
                    });
                });
            });

        SaveChanges();
    }

    public void SaveTurn(World world)
    {
        Territories.ToList().ForEach(territory =>
        {
            var engineTerritory = world.Territories[territory.Id];
            var settler = engineTerritory.HqSettler;
            var owner = engineTerritory.Owner;

            if (owner != null) {
                territory.Owner = Players.First(player => player.Id == owner.Id);
            }

            if (settler != null && territory.Headquarter == null) {
                var hq = new HeadQuarter { Name = "New Headquarter", Settler = Players.First(player => player.Id == settler.Id) };
                HeadQuarters.Add(hq);
                territory.Headquarter = hq;
            }

            territory.Constructs = engineTerritory.Constructs.Select(construct => new Construct { Name = Enum.GetName(typeof(engine.Models.Construct), construct)! }).ToList();
            territory.Armies = engineTerritory.Units.Armies;
            territory.Cavalries = engineTerritory.Units.Cavalries;
            territory.Heavies = engineTerritory.Units.Heavies;
            territory.Spies = engineTerritory.Units.Spies;
            territory.Mines = engineTerritory.Mines;
            territory.Loyalty = engineTerritory.Loyalty;
        });

        Players.ToList().ForEach(player => player.InfluencePoints = world.Players.First(p => player.Id == p.Id).InfluencePoints);

        SaveChanges();
    }
}