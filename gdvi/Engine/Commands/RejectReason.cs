namespace gdvi.Engine.Commands;

public enum RejectReason
{
    BuildingHqTooCloseToAnotherPlayerBuildingHq,
    BuildingHqTooCloseToExistingHq,
    BuildingHqOnOccupiedTerritory,
    BuildingMultipleHqs,
    BuildingHqWhenPlayerAlreadyHasHq,
    SpawningOnInvalidTerritory,
    SpawningTooManyArmies,
    SpawningNegativeArmies,
}