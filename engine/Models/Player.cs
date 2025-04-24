using System.ComponentModel.DataAnnotations;

namespace engine.Models;

public class Player
{

    public string Colour = "#fff";
    [Key] public required Guid Id;
    public int InfluencePoints = 0;
    public List<Item> Inventory = [];
    public required string Name;
}