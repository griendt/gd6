namespace gdvi.Models;

public class Player
{
    public int Id;
    public required Territory Hq;
    public required string Color;
    public List<Item> Inventory = [];
}