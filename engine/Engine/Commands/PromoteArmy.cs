using engine.Models;

namespace engine.Engine.Commands;

public class PromoteArmy : Command, IHasOrigin
{
    public required int Quantity;
    public required Unit UnitType;

    public required Territory Origin { get; set; }
    public override Phase Phase() => Engine.Phase.Construction;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].Owner = Issuer;

        foreach (var _ in Enumerable.Range(0, Quantity)) {
            Issuer.InfluencePoints -= UnitType.PromotionCost();
            world.Territories[Origin.Id].Units.Pop(Unit.Army);
            world.Territories[Origin.Id].Units.Add(UnitType);
        }
    }

    [Validator]
    public static void ValidateOwnership(List<PromoteArmy> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.Origin.Owner != promotion.Issuer)
            .Each(promotion => promotion.Reject(RejectReason.PlayerDoesNotOwnOriginTerritory));
    }

    [Validator]
    public static void ValidateQuantityIsPositive(List<PromoteArmy> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.Quantity < 0)
            .Each(promotion => promotion.Reject(RejectReason.PromotingNegativeQuantity));
    }

    [Validator]
    public static void ValidateSufficientArmiesInOrigin(List<PromoteArmy> promotions, World world)
    {
        promotions
            .GroupBy(promotion => promotion.Origin)
            .Where(group => group.Sum(promotion => promotion.Quantity) > group.Key.Units.Armies)
            .Each(group => group.Each(promotion => promotion.Reject(RejectReason.InsufficientArmies, group)));
    }

    [Validator]
    public static void ValidateSufficientInfluencePoints(List<PromoteArmy> promotions, World world)
    {
        promotions
            .GroupBy(promotion => promotion.Issuer)
            .Where(group => group.Sum(promotion => promotion.UnitType.PromotionCost() * promotion.Quantity) > group.Key.InfluencePoints)
            .Each(group => group.Each(promotion => promotion.Reject(RejectReason.InsufficientInfluencePoints, group)));
    }

    [Validator]
    public static void ValidateCloseEnoughToOwnedHq(List<PromoteArmy> promotions, World world)
    {
        promotions
            .Where(promotion => !promotion.Origin.ContainsHq)
            .Where(promotion => !promotion.Origin.Neighbours().Any(neighbour => neighbour.ContainsHq && neighbour.Owner == promotion.Issuer))
            .Each(promotion => promotion.Reject(RejectReason.PromotingTooFarFromOwnedHq));
    }

    [Validator]
    public static void ValidatePromoteToValidType(List<PromoteArmy> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.UnitType == Unit.Army)
            .Each(promotion => promotion.Reject(RejectReason.InvalidPromotionUnitType));
    }

    [Validator]
    public static void ValidateMinimumPromotionLoyalty(List<PromoteArmy> promotions, World world)
    {
        promotions
            .Where(promotion => promotion.UnitType.MinimumPromotionLoyalty() > promotion.Origin.Loyalty)
            .Each(promotion => promotion.Reject(RejectReason.PromotingInTooLowLoyaltyTerritory));
    }
}