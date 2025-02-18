using gdvi.Models;

namespace gdvi.Engine.Commands;

public class CreateHq : LocalizedCommand
{
    public override Phase Phase() => Engine.Phase.Construction;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].HqSettler = Issuer;
        world.Territories[Origin.Id].Owner = Issuer;
    }

    public new static void Validate<T>(IEnumerable<T> commands, World world)
    {
        if (commands.Where(command => command is CreateHq).ToList() is not List<CreateHq> createHqCommands) {
            return;
        }
        
        ValidateBuildingHqTooCloseToAnotherPlayerBuildingHq(createHqCommands, world);
        ValidateBuildingHqTooCloseToExistingHq(createHqCommands, world);
    }

    private static void ValidateBuildingHqTooCloseToAnotherPlayerBuildingHq(List<CreateHq> commands, World world)
    {
        foreach (var command in commands) {
            var borders = world.TerritoryBorders[command.Origin.Id].ToHashSet();
            borders.Add(command.Origin.Id);

            var conflicts = commands
                .Where(other => 
                    other != command && 
                    
                    world.TerritoryBorders[other.Origin.Id].ToHashSet().Intersect(borders).Any())
                .ToList();

            if (conflicts.Count > 0) {
                command.Reject(CommandRejection.BuildingHqTooCloseToAnotherPlayerBuildingHq, conflicts);
            }
        }
    }

    private static void ValidateBuildingHqTooCloseToExistingHq(List<CreateHq> commands, World world)
    {
        var existingHqIds = world.Territories
            .Where(kv => kv.Value.HqSettler != null)
            .Select(kv => kv.Key)
            .ToList();

        var blacklistedTerritoryIds = new HashSet<int>(existingHqIds);

        foreach (var range in Enumerable.Range(0, 2)) {
            var blacklistBorder = blacklistedTerritoryIds
                .SelectMany(id => world.Territories[id]
                    .Neighbours()
                    .Select(territory => territory.Id))
                .ToList();

            blacklistedTerritoryIds.UnionWith(blacklistBorder);
        }
        
        commands
            .Where(command => blacklistedTerritoryIds.Contains(command.Origin.Id))
            .Each(command => command.Reject(CommandRejection.BuildingHqTooCloseToExistingHq, []));
    }
}