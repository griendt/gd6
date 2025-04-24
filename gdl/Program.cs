using engine.Models;
using gdl;
using web;

var world = new World();
var parser = new GdlParser(world);
var gdl = File.ReadAllText("/home/alex/projects/gd6/gdl/gd6.gdl");
var db = new Gd6DbContext();

parser.Parse(gdl);

foreach (var turn in parser.Turns()) {
    turn.Process();
    Console.WriteLine("Turn processed");
}


db.FromWorld(world);


Console.WriteLine("Hello, World!");