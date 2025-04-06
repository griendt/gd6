using engine.Engine.Commands;
using engine.Engine.MoveResolutions;
using engine.Models;

namespace engine.Engine;

public static class MoveArmyOrderResolver
{
    /// <summary>
    ///     Executes one step of resolution. Returns `true` if at least one
    ///     move has been resolved, `false` otherwise. Note that this method
    ///     can and will mutate the commands in-place.
    /// </summary>
    /// <param name="commands">The commands to process.</param>
    /// <param name="world">The world to process the commands in.</param>
    /// <returns>Whether any resolution has been done.</returns>
    public static bool Resolve(List<MoveArmy> commands, World world)
    {
        return ResolveBasicSkirmish(commands, world) || ResolveMutualInvasion(commands, world);
    }

    private static bool ResolveBasicSkirmish(List<MoveArmy> commands, World world)
    {
        var isResolutionDone = false;

        // If multiple players attempt to attack the same territory, it is a skirmish.
        commands
            .Where(command => !command.IsProcessed)
            .GroupBy(command => command.Path[1])
            .Each(group =>
            {
                // Skirmish of two or more players moving to the same target.
                // FIXME: if it is two players and one of them owns the target already,
                // then the Distribution surely should come first, and it should be an invasion!
                if (group.DistinctBy(command => command.Issuer).Count() > 1) {
                    new Skirmish().Resolve(group.ToList(), world);
                    isResolutionDone = true;
                }
            });

        return isResolutionDone;
    }

    private static bool ResolveMutualInvasion(List<MoveArmy> commands, World world)
    {
        var isResolutionDone = false;

        commands
            .Where(command => !command.IsProcessed)
            .GroupBy(command => [command.Path[0].Id, command.Path[1].Id], HashSet<int>.CreateSetComparer())
            .Each(group =>
            {
                // It is assumed that it is impossible for two different players to have a valid move A→B
                // here simultaneously. Validation should take care of this, and longer paths should be
                // resolved in such a way that this situation should not occur.
                // FIXME: what if one player makes a move A→B and a move B→A at the same time?
                new Skirmish().Resolve(group.ToList(), world);
                isResolutionDone = true;
            });

        return isResolutionDone;
    }
}