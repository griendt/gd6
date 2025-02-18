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

        foreach (var group in createHqCommands.GroupBy(command => command.Origin)) {
            if (group.Count() <= 1) {
                continue;
            }

            group.Each(command => command.Reject(CommandRejection.BuildingHqInSameTerritoryAsAnotherPlayer, group));
        }

        var existingHqIds = world.Territories
            .Where(kv => kv.Value.HqSettler != null)
            .Select(kv => kv.Key)
            .ToList();

        var blacklistedTerritoryIds = new HashSet<int>(existingHqIds);

        foreach (var range in Enumerable.Range(0, 2)) {
            var blacklistBorder = blacklistedTerritoryIds
                .SelectMany(id => world.Territories[id]
                    .Neighbours()
                    .Select(territory => territory.Id));

            blacklistedTerritoryIds.UnionWith(blacklistBorder);
        }

        var groups = createHqCommands.GroupBy(command => command.Origin);

        return;
    }
}