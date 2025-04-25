using engine.Models;
using gdl;
using web;

var world = new World();
var parser = new GdlParser(world);
var gdl = File.ReadAllText("/home/alex/projects/gd6/gdl/gd6.gdl");
var db = new Gd6DbContext();

// Mutates world, so must be done before creating a new Game entry in the database.
parser.Parse(gdl);

// Initialize players and territories if the db is still empty
if (!db.Players.Any()) {
    db.FromWorld(world);
}

// Process the turns
foreach (var turn in parser.Turns()) {
    turn.Process();
    Console.WriteLine("Turn processed");

    db.SaveTurn(world);
}