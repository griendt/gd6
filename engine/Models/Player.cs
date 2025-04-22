using System.ComponentModel.DataAnnotations;

namespace engine.Models;

public class Player
{
    [Key] public required Guid Id;
    public required string Name;
    public List<Item> Inventory = [];
    public int InfluencePoints = 0;
}