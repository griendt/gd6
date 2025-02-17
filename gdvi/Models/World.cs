namespace gdvi.Models;

public class World
{
    public readonly Dictionary<int, Territory> Territories = [];
    public readonly Dictionary<int, List<int>> TerritoryBorders = [];

    public void AddBorder(Territory first, Territory second)
    {
        TerritoryBorders.SetOrAppend(first.Id, second.Id);
        TerritoryBorders.SetOrAppend(second.Id, first.Id);
    }
}