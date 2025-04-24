using engine.Models;
using gdl;

var world = new World();
var parser = new GdlParser(world);
var gdl = File.ReadAllText("/home/alex/projects/gd6/gdl/gd6.gdl");

parser.Parse(gdl);

foreach (var turn in parser.Turns()) {
    turn.Process();
    Console.WriteLine("Turn processed");
}


Console.WriteLine("Hello, World!");