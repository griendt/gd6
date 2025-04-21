using engine.Models;

namespace engine.Engine.Commands;

public class CreateFortress : Command, IHasOrigin
{
    public required Territory Origin { get; set; }

    public override Phase Phase() => Engine.Phase.Construction;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].Constructs.Add(Construct.Fortress);
    }

    [Validator]
    public static void ValidateIssuerMustBeOwner(List<CreateFortress> commands, World world)
    {
        commands
            .Where(command => command.Issuer != command.Origin.Owner)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidateTargetDoesNotContainConstruct(List<CreateFortress> commands, World world)
    {
        commands
            .Where(command => command.Origin.Constructs.Count > 0)
            .Each(command => command.Reject(RejectReason.TargetAlreadyContainsConstruct));
    }

    [Validator]
    public static void ValidateIssuerHasEnoughInfluencePointers(List<CreateFortress> commands, World world)
    {
        // Note: this does not take into account other command types that also cost IP!
        // There should be some sort of step during execution that checks for IP.
        commands
            .GroupBy(command => command.Issuer)
            .Where(group => group.Key.InfluencePoints < 20 * group.Count())
            .Each(group => group.Each(command => command.Reject(RejectReason.InsufficientInfluencePoints, group.ToList())));
    }

    [Validator]
    public static void ValidateOnlyOneFortressPerTerritory(List<CreateFortress> commands, World world)
    {
        commands
            .GroupBy(command => command.Origin)
            .Where(group => group.Count() > 1)
            .Each(group => group.Each(command => command.Reject(RejectReason.BuildingMultipleConstructsInOneTerritory, group.ToList())));
    }
}