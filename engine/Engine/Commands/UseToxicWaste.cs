using engine.Models;

namespace engine.Engine.Commands;

public class UseToxicWaste : InventoryCommand, IHasOrigin, IHasTarget
{
    public required Territory Origin { get; set; }
    public required Territory Target { get; set; }
    protected override Item ItemType() => Item.ToxicWaste;
    
    public override void Process(World world)
    {
        // TODO: Maybe also remove existing constructs?
        Target.IsWasteland = true;
    }
    
    [Validator]
    public static void ValidatePlayerOwnsOriginTerritory(IEnumerable<UseToxicWaste> commands, World world)
    {
        commands
            .Where(command => world.Territories[command.Origin.Id].Owner != command.Issuer)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }
    
    [Validator]
    public static void ValidateOriginAdjacentToTarget(IEnumerable<UseToxicWaste> commands, World world)
    {
        commands
            .Where(command => !world.TerritoryBorders[command.Origin.Id].Contains(command.Target.Id))
            .Each(command => command.Reject(RejectReason.TargetNotAdjacentToOrigin));
    }
}