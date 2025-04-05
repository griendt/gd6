using System.ComponentModel.DataAnnotations;

namespace engine.Models;

public class Player
{
    public string Color = "#000";
    public Territory? Hq = null;

    [Key] public required int Id;

    public List<Item> Inventory = [];

    public required string Name;
}