using engine.Models;

namespace engine.Engine.Commands;

public class UseCropSupply : InventoryCommand
{
    protected override Item ItemType() => Item.CropSupply;

    public required IDictionary<int, int> Quantities;

    public override void Process(World world)
    {
        foreach (var (territoryId, quantity) in Quantities) {
            world.Territories[territoryId].Units.AddArmies(quantity);
        }
    }

    [Validator]
    public static void ValidateTotalQuantity(List<UseCropSupply> commands, World world)
    {
        commands
            .Where(command => command.Quantities.Values.Sum() > Math.Max(5, SpawnArmy.MaxArmiesAllowedToSpawn(command.Issuer, world)))
            .Each(command => command.Reject(RejectReason.SpawningTooManyArmies));
    }
}