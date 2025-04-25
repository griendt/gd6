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
    public DbSet<Player> Players { get; set; }
    public DbSet<Territory> Territories { get; set; }
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

        SaveChanges();
    }

    public void SaveTurn(World world)
    {
        Territories.ToList().ForEach(territory =>
        {
            var settler = world.Territories[territory.Id].HqSettler;
            var owner = world.Territories[territory.Id].Owner;

            if (owner != null) {
                territory.Owner = Players.First(player => player.Id == owner.Id);
            }

            if (settler != null && territory.Headquarter == null) {
                var hq = new HeadQuarter { Name = "New Headquarter", Settler = Players.First(player => player.Id == settler.Id) };
                HeadQuarters.Add(hq);
                territory.Headquarter = hq;
            }
        });

        SaveChanges();
    }
}