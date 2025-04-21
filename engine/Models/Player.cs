using System.ComponentModel.DataAnnotations;

namespace engine.Models;

public class Player
{
    public string Color = "#000";
    public Territory? Hq = null;
    [Key] public required int Id;
    public required string Name;
    public List<Item> Inventory = [];
    public int InfluencePoints = 0;
}