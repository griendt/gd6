using engine.Models;

namespace engine.Engine.Commands;

public class PromoteArmyToCavalry : Command, IHasOrigin
{
    public required int Quantity;
    public required Territory Origin { get; set; }
    public override Phase Phase() => Engine.Phase.Construction;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].Owner = Issuer;

        foreach (var _ in Enumerable.Range(0, Quantity)) {
            Issuer.InfluencePoints -= 3;
            world.Territories[Origin.Id].Units.Pop(Unit.Army);
            world.Territories[Origin.Id].Units.AddCavalry();
        }
    }

    [Validator]
    public static void ValidateOwnership(List<PromoteArmyToCavalry> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.Origin.Owner != promotion.Issuer)
            .Each(promotion => promotion.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidateQuantityIsPositive(List<PromoteArmyToCavalry> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.Quantity < 0)
            .Each(promotion => promotion.Reject(RejectReason.PromotingNegativeQuantity));
    }

    [Validator]
    public static void ValidateSufficientArmiesInOrigin(List<PromoteArmyToCavalry> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.Quantity > promotion.Origin.Units.Armies)
            .Each(promotion => promotion.Reject(RejectReason.InsufficientArmies));
    }

    [Validator]
    public static void ValidateCloseEnoughToOwnedHq(List<PromoteArmyToCavalry> promotions, World world)
    {
        promotions
            .Where(promotion => !promotion.Origin.ContainsHq)
            .Where(promotion => !promotion.Origin.Neighbours().Any(neighbour => neighbour.ContainsHq && neighbour.Owner == promotion.Issuer))
            .Each(promotion => promotion.Reject(RejectReason.PromotingTooFarFromOwnedHq));
    }

}