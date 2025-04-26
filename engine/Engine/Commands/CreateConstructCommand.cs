using System.Collections.Immutable;
using engine.Models;

namespace engine.Engine.Commands;

public abstract class CreateConstructCommand : Command, IHasOrigin
{
    protected abstract Construct ConstructType();
    protected abstract int Cost(World world);
    
    public required Territory Origin { get; set; }

    public sealed override Phase Phase() => Engine.Phase.Construction;
    
    
    public override void Process(World world)
    {
        // TODO: add tests asserting that IP are deducted
        Issuer.InfluencePoints -= Cost(world);
        world.Territories[Origin.Id].Constructs.Add(ConstructType());
    }
    
    [Validator]
    public static void ValidateIssuerMustBeOwner(IEnumerable<CreateConstructCommand> commands, World world)
    {
        commands
            .Where(command => command.Issuer != command.Origin.Owner)
            .Each(command => command.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidateTargetDoesNotContainConstruct(IEnumerable<CreateConstructCommand> commands, World world)
    {
        commands
            .Where(command => command.Origin.Constructs.Count > 0)
            .Each(command => command.Reject(RejectReason.TargetAlreadyContainsConstruct));
    }

    [Validator]
    public static void ValidateIssuerHasEnoughInfluencePoints(IEnumerable<CreateConstructCommand> commands, World world)
    {
        commands
            .GroupBy(command => command.Issuer)
            .Where(group => group.Key.InfluencePoints < group.Sum(command => command.Cost(world)))
            .Each(group => group.Each(command => command.Reject(RejectReason.InsufficientInfluencePoints, group.ToList())));
    }

    [Validator]
    public static void ValidateOnlyOneConstructPerTerritory(IEnumerable<CreateConstructCommand> commands, World world)
    {
        commands
            .GroupBy(command => command.Origin)
            .Where(group => group.Count() > 1)
            .Each(group => group.Each(command => command.Reject(RejectReason.BuildingMultipleConstructsInOneTerritory, group.ToList())));
    }

    [Validator]
    public static void ValidateTargetIsNotWasteland(IEnumerable<CreateConstructCommand> commands, World world)
    {
        commands
            .Where(command => command.Origin.IsWasteland)
            .Each(command => command.Reject(RejectReason.BuildingOnToxicWasteland));
    }
}