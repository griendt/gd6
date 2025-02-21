using gdvi.Models;

namespace gdvi.Engine.Commands;

public class UseCropSupply : InventoryCommand
{
    protected override Type ItemType() => typeof(CropSupply);

    public required IDictionary<int, int> Quantities;

    public override void Process(World world)
    {
        foreach (var (territoryId, quantity) in Quantities) {
            world.Territories[territoryId].Units.AddArmies(quantity);
        }
    }
}