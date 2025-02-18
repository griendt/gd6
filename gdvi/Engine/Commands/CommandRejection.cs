namespace gdvi.Engine.Commands;

public enum CommandRejection
{
    BuildingHqTooCloseToAnotherPlayerBuildingHq,
    BuildingHqTooCloseToExistingHq,
    BuildingHqOnOccupiedTerritory,
    BuildingMultipleHqs,
    BuildingHqWhenPlayerAlreadyHasHq,
}