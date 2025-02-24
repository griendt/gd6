using gdvi.Models;

namespace gdvi.Engine.Commands;

public class MoveArmy : Command, IHasOrigin, IHasTarget
{
    public required Territory Origin { get; set; }
    public required Territory Target { get; set; }
    public override Phase Phase() => Engine.Phase.Movement;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].Units.Pop();
        world.Territories[Target.Id].Units.AddArmy();
        world.Territories[Target.Id].Owner = Issuer;
    }

    [Validator]
    public static void ValidatePlayerOwnsOriginTerritoryAndHasUnit(IEnumerable<MoveArmy> commands, World world)
    {
        // TODO: for units that can move multiple spaces, this validator is inaccurate for second and subsequent moves!

        commands
            .Where(command => false
                || world.Territories[command.Origin.Id].Owner != command.Issuer
                || world.Territories[command.Origin.Id].Units.Armies <= 0)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidateOriginAdjacentToTarget(IEnumerable<MoveArmy> commands, World world)
    {
        commands
            .Where(command => !world.TerritoryBorders[command.Origin.Id].Contains(command.Target.Id))
            .Each(command => command.Reject(RejectReason.TargetNotAdjacentToOrigin));
    }
}