using System.Diagnostics.Contracts;

namespace engine.Models;

public class World
{
    public Dictionary<int, Territory> Territories = [];
    public Dictionary<int, List<int>> TerritoryBorders = [];
    public List<Player> Players = [];

    [Pure]
    public int NumberOfTerritories(Player player)
    {
        return Territories.Values.Count(territory => territory.Owner == player);
    }

    public void AddBorder(Territory first, Territory second)
    {
        TerritoryBorders.SetOrAppend(first.Id, second.Id);
        TerritoryBorders.SetOrAppend(second.Id, first.Id);
    }

    [Pure]
    public List<int> GetLongestConcurrentlyOccupyingTerritories(Player commandIssuer)
    {
        var territories = Territories.Values
            .Where(territory => territory.Owner == commandIssuer)
            .ToList();

        if (territories.Count == 0) {
            return [];
        }

        var longestInterval = territories
            .Select(territory => territory.Loyalty)
            .Max();

        return territories
            .Where(territory => territory.Loyalty >= longestInterval)
            .Select(territory => territory.Id)
            .ToList();
    }
}