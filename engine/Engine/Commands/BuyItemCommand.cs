using System.ComponentModel;
using engine.Models;

namespace engine.Engine.Commands;

public class BuyItemCommand : Command
{
    public required Func<Item> ItemType;

    public int Cost()
    {
        var itemType = ItemType();

        return itemType switch
        {
            Item.CropSupply => 30,
            Item.Dynamite or Item.ToxicWaste => 20,
            _ => throw new ArgumentOutOfRangeException(),
        };
    }
    
    public sealed override Phase Phase() => Engine.Phase.Inventory;
    
    public override void Process(World world)
    {
        Issuer.Inventory.Add(ItemType());
    }
    
    [Validator]
    public static void ValidatePlayerHasEnoughInfluencePoints(IEnumerable<BuyItemCommand> commands, World world)
    {
        commands
            .GroupBy(command => command.Issuer)
            .Where(group => group.Sum(command => command.Cost()) > group.Key.InfluencePoints)
            .Each(group => group
                .Each(command => command.Reject(RejectReason.InsufficientInfluencePoints, group.ToList()))
            );
    }
}