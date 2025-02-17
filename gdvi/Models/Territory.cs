namespace gdvi.Models;

public class Territory(World world)
{
    public required int Id;
    public Player? Owner = null;
    public Player? HqSettler = null;
    public bool IsWasteland = false;
    public Dictionary<Unit, int> Units = [];

    private IEnumerable<Territory> Neighbours() => world.TerritoryBorders.GetValueOrDefault(Id, []).Select(id => world.Territories[id]);
    public bool IsNeighbour(Territory other) => Neighbours().Contains(other);
}