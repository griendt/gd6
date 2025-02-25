using gdvi.Models;

namespace gdvi.Engine.Commands;

public class SpawnArmy : Command, IHasOrigin
{
    public override Phase Phase() => Engine.Phase.Construction;

    public required int Quantity;

    public static int MaxArmiesAllowedToSpawn(Player player, World world) => 2 + world.NumberOfTerritories(player) / 3;

    public required Territory Origin { get; set; }
    
    public override void Process(World world)
    {
        world.Territories[Origin.Id].Units.AddArmies(Quantity);
    }

    [Validator]
    public static void ValidateSpawningTooManyArmies(List<SpawnArmy> commands, World world)
    {
        foreach (var commandsByPlayer in commands.GroupBy(command => command.Issuer)) {
            var maxSpawns = MaxArmiesAllowedToSpawn(commandsByPlayer.Key, world);

            if (commandsByPlayer.Select(command => command.Quantity).Sum() <= maxSpawns) {
                continue;
            }

            foreach (var command in commandsByPlayer) {
                var conflicts = commandsByPlayer
                    .Where(other => other != command)
                    .ToList();

                command.Reject(RejectReason.SpawningTooManyArmies, conflicts);
            }
        }
    }
    
    [Validator]
    public static void ValidateSpawningNegativeArmies(List<SpawnArmy> commands, World world)
    {
        commands
            .Where(command => command.Quantity < 0)
            .Each(command => command.Reject(RejectReason.SpawningNegativeArmies));
    }
    
    [Validator]
    public static void SpawningTooFarFromOwnedHqs(List<SpawnArmy> commands, World world)
    {
        var validSpawnLocationsByPlayer = new Dictionary<Player, List<int>>();
        var territoriesWithHq = world.Territories.Values
            .Where(territory => territory.ContainsHq && !territory.IsNeutral);
        
        foreach (var territory in territoriesWithHq) {
            validSpawnLocationsByPlayer.SetOrAppend(territory.Owner!, territory.Id);
            territory.Neighbours()
                .Where(neighbour => neighbour.Owner == null || neighbour.Owner == territory.Owner)
                .Each(neighbour => validSpawnLocationsByPlayer[territory.Owner!].Add(neighbour.Id));
        }

        foreach (var command in commands) {
            var validSpawnIds = validSpawnLocationsByPlayer.GetValueOrDefault(command.Issuer, []);

            if (validSpawnIds.Count == 0) {
                var fallbackSpawnIds = world.GetLongestConcurrentlyOccupyingTerritories(command.Issuer);

                if (fallbackSpawnIds.Count == 0) {
                    command.Reject(RejectReason.NoValidSpawnLocations);
                }
                else if (!fallbackSpawnIds.Contains(command.Origin.Id)) {
                    command.Reject(RejectReason.SpawningNotInLongestConcurrentlyOccupyingTerritory);
                }
            }
            else if (!validSpawnIds.Contains(command.Origin.Id)) {
                command.Reject(RejectReason.SpawningTooFarFromOwnHq);
            }
        }
    }


}