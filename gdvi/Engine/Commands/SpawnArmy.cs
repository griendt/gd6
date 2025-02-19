using gdvi.Models;

namespace gdvi.Engine.Commands;

public class SpawnArmy : LocalCommand
{
    public override Phase Phase() => Engine.Phase.Construction;

    public required int Quantity;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].Units.AddArmies(Quantity);
    }

    [Validator]
    public static void ValidateSpawningTooManyArmies(List<SpawnArmy> commands, World world)
    {
        foreach (var commandsByPlayer in commands.GroupBy(command => command.Issuer)) {
            var maxSpawns = 2 + world.NumberOfTerritories(commandsByPlayer.Key) / 3;

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
            if (!validSpawnLocationsByPlayer.TryGetValue(command.Issuer, out var validSpawnIds)) {
                // What if you do not own any HQ anymore? Right now we reject, but this makes people die super fast.
                command.Reject(RejectReason.SpawningOnInvalidTerritory);
            }

            else if (!validSpawnIds.Contains(command.Origin.Id)) {
                command.Reject(RejectReason.SpawningOnInvalidTerritory);
            }
        }
    }
}