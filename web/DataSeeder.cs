using engine;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace web;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method)]
public class SeedAttribute : Attribute;

public class DataSeeder(Gd6DbContext db)
{
    public void Seed(DbContext context, bool seed)
    {
        GetType()
            .GetMethods()
            .Where(method => method.GetCustomAttributes(typeof(SeedAttribute), false).Length > 0)
            .Each(method => method.Invoke(this, []));
    }

    [Seed]
    public void SeedTerritories()
    {
        if (db.Territories.Any()) {
            return;
        }

        db.Territories.Add(new Territory
        {
            Identifier = "1",
            Coordinates = [
                new Coordinate { X = 50, Y = 50 }, 
                new Coordinate { X = 80, Y = 140 }, 
                new Coordinate { X = 100, Y = 170 }, 
                new Coordinate { X = 140, Y = 160 },
                new Coordinate { X = 160, Y = 150 },
                new Coordinate { X = 200, Y = 40 },
                new Coordinate { X = 150, Y = 50 },
            ],
        });

        db.SaveChanges();
    }
}