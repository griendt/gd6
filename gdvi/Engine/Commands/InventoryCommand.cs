using System.Collections.Immutable;
using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class InventoryCommand : Command
{
    protected abstract Item ItemType();

    public sealed override Phase Phase() => Engine.Phase.Inventory;
    
    [Validator]
    public static void ValidatePlayerHasItem(IEnumerable<InventoryCommand> commands, World world)
    {
        commands
            .GroupBy(command => (command.ItemType(), command.Issuer))
            .Each(group =>
            {
                var issuer = group.Key.Issuer;
                var numberAvailable = issuer
                    .Inventory
                    .Count(item => item == group.Key.Item1);

                if (numberAvailable >= group.Count()) {
                    return;
                }
                
                foreach (var command in group) {
                    command.Reject(
                        RejectReason.InsufficientAmountOfItems, 
                        group.Where(other => other != command));
                }
            });
    }

    [Validator]
    public static void ValidatePlayerIsPresentOnMap(IEnumerable<InventoryCommand> commands, World world)
    {
        commands
            .Where(command => world.NumberOfTerritories(command.Issuer) == 0)
            .Each(command => command.Reject(RejectReason.PlayerMustOwnOneTerritoryToUseItem));
    }
    
}