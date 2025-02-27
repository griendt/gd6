using System.ComponentModel.DataAnnotations;
using System.Text;
using engine;
using Microsoft.EntityFrameworkCore;

namespace web;

public class Gd6DbContext : DbContext
{
    public DbSet<Territory> Territories { get; set; }

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
        
        foreach (var entry in changedEntities)
        {
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

    [MinLength(3)]
    public List<Coordinate> Coordinates { get; init; } = [];
}

public class Coordinate
{
    public int X { get; set; }
    public int Y { get; set; }
}