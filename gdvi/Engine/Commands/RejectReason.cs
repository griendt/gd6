namespace gdvi.Engine.Commands;

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
}