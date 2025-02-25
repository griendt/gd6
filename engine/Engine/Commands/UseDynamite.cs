using gdvi.Models;

namespace gdvi.Engine.Commands;

public class UseDynamite : InventoryCommand, IHasOrigin, IHasTarget
{
    public required Territory Origin { get; set; }
    public required Territory Target { get; set; }
    protected override Item ItemType() => Item.Dynamite;
    public override void Process(World world) => Target.ApplyDynamitePenalty();
    
    [Validator]
    public static void ValidatePlayerOwnsOriginTerritory(IEnumerable<UseDynamite> commands, World world)
    {
        commands
            .Where(command => world.Territories[command.Origin.Id].Owner != command.Issuer)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }
    
    [Validator]
    public static void ValidateOriginAdjacentToTarget(IEnumerable<UseDynamite> commands, World world)
    {
        commands
            .Where(command => !world.TerritoryBorders[command.Origin.Id].Contains(command.Target.Id))
            .Each(command => command.Reject(RejectReason.TargetNotAdjacentToOrigin));
    }
}