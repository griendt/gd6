using engine;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using web.Models;

namespace web;

public class DataSeeder(Gd6DbContext db)
{
    private readonly Game _game = new() { Name = "Global Domination VI" };

    public void Seed(DbContext context, bool seed)
    {
        db.Games.Add(_game);

        List<Action> seeders = [
            SeedPlayers,
            SeedPlayersToGame,
            SeedTerritories,
        ];

        seeders.ForEach(seeder =>
        {
            seeder();
            db.SaveChanges();
        });
    }

    private void SeedPlayers()
    {
        if (db.Players.Any()) {
            return;
        }

        List<Player> players =
        [
            new() { Name = "Aluce", Colour = "#f00" },
            new() { Name = "Psycho17", Colour = "#0f0" },
        ];

        players.ForEach(player => db.Players.Add(player));
    }

    private void SeedTerritories()
    {
        if (db.Territories.Any()) {
            return;
        }

        db.Territories.Add(new Territory
        {
            Game = _game,
            Identifier = "1",
            Coordinates =
            [
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
            Game = _game,
            Identifier = "2",
            Coordinates = [
                new Coordinate { X = 100, Y = 170 }, 
                new Coordinate { X = 90, Y = 220 }, 
                new Coordinate { X = 140, Y = 230 }, 
                new Coordinate { X = 200, Y = 330 }, 
                new Coordinate { X = 210, Y = 300 }, 
                new Coordinate { X = 240, Y = 190 }, 
                new Coordinate { X = 250, Y = 150 }, 
                new Coordinate { X = 160, Y = 150 }, 
            ],
        });
        
        db.Territories.Add(new Territory
        {
            Game = _game,
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

        db.Territories.Add(new Territory
        {
            Game = _game,
            Identifier = "4",
            Coordinates =
            [
                new Coordinate { X = 370, Y = 70 },
                new Coordinate { X = 420, Y = 60 },
                new Coordinate { X = 440, Y = 100 },
                new Coordinate { X = 430, Y = 130 },
                new Coordinate { X = 420, Y = 160 },
                new Coordinate { X = 460, Y = 170 },
                new Coordinate { X = 430, Y = 190 },
                new Coordinate { X = 420, Y = 230 },
                new Coordinate { X = 350, Y = 260 },
                new Coordinate { X = 340, Y = 180 },
                new Coordinate { X = 350, Y = 150 },
                new Coordinate { X = 360, Y = 100 },
            ],
        });

        db.Territories.Add(new Territory
        {
            Game = _game,
            Identifier = "5",
            Coordinates =
            [
                new Coordinate { X = 250, Y = 150 },
                new Coordinate { X = 340, Y = 180 },
                new Coordinate { X = 350, Y = 260 },
                new Coordinate { X = 350, Y = 260 },
                new Coordinate { X = 340, Y = 330 },
                new Coordinate { X = 370, Y = 380 },
                new Coordinate { X = 320, Y = 400 },
                new Coordinate { X = 280, Y = 380 },
                new Coordinate { X = 210, Y = 300 },
            ],
        });
        
        db.Territories.Add(new Territory
        {
            Game = _game,
            Identifier = "6",
            Coordinates =
            [
                new Coordinate { X = 90, Y = 220 }, 
                new Coordinate { X = 60, Y = 320 },
                new Coordinate { X = 80, Y = 420 },
                new Coordinate { X = 200, Y = 330 }, 
                new Coordinate { X = 140, Y = 230 }, 
            ],
        });
    }

    private void SeedPlayersToGame()
    {
        foreach (var player in db.Players) {
            db.GamePlayers.Add(new GamePlayer
            {
                Game = _game,
                Player = player,
            });
        }
    }
}