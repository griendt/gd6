namespace engine.Engine.Commands;

public enum RejectReason
{
    BuildingHqTooCloseToAnotherPlayerBuildingHq,
    BuildingHqTooCloseToExistingHq,
    BuildingHqOnOccupiedTerritory,
    BuildingMultipleHqs,
    BuildingHqWhenPlayerAlreadyHasHq,
    SpawningTooFarFromOwnHq,
    SpawningNotInLongestConcurrentlyOccupyingTerritory,
    SpawningTooManyArmies,
    SpawningNegativeArmies,
    NoValidSpawnLocations,
    InsufficientAmountOfItems,
    PlayerMustOwnOneTerritoryToUseItem,
    PlayerDoesNotOwnOriginTerritory,
    TargetNotAdjacentToOrigin,
    PathNotConnected,
    InvalidPathLength,
    InvalidPathStartingPoint,
    TargetAlreadyContainsConstruct,
    InsufficientInfluencePoints,
    BuildingMultipleConstructsInOneTerritory,
    BuildingOnToxicWasteland,
    InsufficientArmies,
    PromotingNegativeQuantity,
    PromotingTooFarFromOwnedHq,
    InvalidPromotionUnitType,
    PromotingInTooLowLoyaltyTerritory,
}