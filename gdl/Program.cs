using engine.Models;
using gdl;
using Microsoft.EntityFrameworkCore;
using web;

var world = new World();
var parser = new GdlParser(world);
var gdl = File.ReadAllText("/files/gd6.gdl");
var db = new Gd6DbContext();

// Mutates world, so must be done before creating a new Game entry in the database.
parser.Parse(gdl);

db.Territories.ExecuteDelete();
db.HeadQuarters.ExecuteDelete();
db.Players.ExecuteDelete();
db.FromWorld(world);

// Process the turns
foreach (var turn in parser.Turns()) {
    turn.Process();
    Console.WriteLine("Turn processed");

    db.SaveTurn(world);
}