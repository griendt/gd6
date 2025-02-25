// See https://aka.ms/new-console-template for more information

using gdvi.Models;

var world = new World();

foreach (var id in Enumerable.Range(1, 5)) {
    world.Territories.Add(id, new Territory(world) { Id = id });

    if (id > 1) {
        world.TerritoryBorders[id] = [id - 1, id + 1];
    }
}

var x = 3;