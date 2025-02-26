using System.ComponentModel.DataAnnotations;

namespace engine.Models;

public class Player
{
    [Key]
    public required int Id;
    
    public Territory? Hq = null;
    public string Color = "#000";
    public List<Item> Inventory = [];
}