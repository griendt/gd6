namespace gdvi.Models;

public class Player
{
    public required int Id;
    public Territory? Hq = null;
    public string Color = "#000";
    public List<Item> Inventory = [];
}