using gdvi.Engine;
using gdvi.Models;

namespace tests;

public class Tests
{
    private World _world;

    [SetUp]
    public void Setup()
    {
        _world = new World();

        foreach (var id in Enumerable.Range(1, 5)) {
            _world.Territories.Add(id, new Territory(_world) { Id = id });

            if (id is <= 1 or > 5) {
                continue;
            }

            _world.AddBorder(_world.Territories[id - 1], _world.Territories[id]);
        }
    }

    [Test]
    public void TestBoundaries()
    {
        foreach (var id in Enumerable.Range(2, 3))
        {
            Assert.Multiple(() =>
            {
                Assert.That(_world.Territories[id].IsNeighbour(_world.Territories[id + 1]));
                Assert.That(_world.Territories[id - 1].IsNeighbour(_world.Territories[id]));
            });
        }
    }

    [Test]
    public void ItRemovesArmyFromWasteland()
    {
        _world.Territories[3].Units.AddArmies(4);
        _world.Territories[3].IsWasteland = true;
        new Turn(_world).Process();
        
        Assert.That(_world.Territories[3].Units.Armies, Is.EqualTo(3));
    }
    
    [Test]
    public void ItDoesNotMakeTerritoryNeutralIfLastUnitIsRemovedByWasteland()
    {
        _world.Territories[3].Owner = new Player
        {
            Id = 1,
            Inventory = [],
            Hq = _world.Territories[3],
            Color = "#f00",
        };
        
        _world.Territories[3].Units.AddArmy();
        _world.Territories[3].IsWasteland = true;
        
        new Turn(_world).Process();
        
        Assert.Multiple(() =>
        {
            Assert.That(_world.Territories[3].Units.Armies, Is.EqualTo(0));
            Assert.That(_world.Territories[3].Owner, Is.Not.Null);
        });
    }
}