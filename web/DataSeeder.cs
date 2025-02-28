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
        
        db.Territories.Add(new Territory
        {
            Identifier = "2",
            Coordinates = [
                new Coordinate { X = 100, Y = 170 }, 
                new Coordinate { X = 90, Y = 220 }, 
                new Coordinate { X = 140, Y = 230 }, 
                new Coordinate { X = 200, Y = 220 }, 
                new Coordinate { X = 210, Y = 300 }, 
                new Coordinate { X = 239, Y = 190 }, 
                new Coordinate { X = 250, Y = 150 }, 
                new Coordinate { X = 160, Y = 150 }, 
            ],
        });
        
        db.Territories.Add(new Territory
        {
            Identifier = "3",
            Coordinates = [
                new Coordinate { X = 160, Y = 150 },
                new Coordinate { X = 200, Y = 40 },
                new Coordinate { X = 220, Y = 20 },
                new Coordinate { X = 250, Y = 30 },
                new Coordinate { X = 260, Y = 40 },
                new Coordinate { X = 280, Y = 10 },
                new Coordinate { X = 290, Y = 60 },
                new Coordinate { X = 330, Y = 50 },
                new Coordinate { X = 360, Y = 40 },
                new Coordinate { X = 370, Y = 70 },
                new Coordinate { X = 360, Y = 100 },
                new Coordinate { X = 350, Y = 150 },
                new Coordinate { X = 340, Y = 180 },
                new Coordinate { X = 250, Y = 150 },
            ],
        });

        db.SaveChanges();
    }
}