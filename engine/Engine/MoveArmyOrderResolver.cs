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
    public static bool Resolve(List<MoveUnit> commands, World world) =>
        ResolveDistributesToOwnedTerritories(commands, world)
        || ResolveDistributesToNeutralTerritories(commands, world)
        || ResolveBasicSkirmish(commands, world)
        || ResolveCircularInvasion(commands, world)
        || ResolveInvasion(commands, world);

    private static bool ResolveDistributesToOwnedTerritories(List<MoveUnit> commands, World world)
    {
        var distributes = commands
            .Where(command => !command.IsProcessed)
            .Where(command => command.Issuer == command.Path.Second().Owner)
            .ToList();

        new Distribute().Resolve(distributes, world);

        return distributes.Count > 0;
    }

    private static bool ResolveDistributesToNeutralTerritories(List<MoveUnit> commands, World world)
    {
        var isResolutionDone = false;

        commands
            .Where(command => !command.IsProcessed)
            .Where(command => command.Path[1].Owner == null)
            .GroupBy(command => command.Path[1])
            .Where(group => group.DistinctBy(command => command.Issuer).Count() == 1)
            .Each(group =>
            {
                new Distribute().Resolve(group.ToList(), world);
                isResolutionDone = true;
            });

        return isResolutionDone;
    }

    private static bool ResolveBasicSkirmish(List<MoveUnit> commands, World world)
    {
        var isResolutionDone = false;

        // If multiple players attempt to attack the same territory, it is a skirmish.
        commands
            .Where(command => !command.IsProcessed)
            .GroupBy(command => command.Path[1])
            .Where(group => group.DistinctBy(command => command.Issuer).Count() > 1)
            .Each(group =>
            {
                new Skirmish().Resolve(group.ToList(), world);
                isResolutionDone = true;
            });

        return isResolutionDone;
    }

    private static bool ResolveCircularInvasion(List<MoveUnit> commands, World world)
    {
        var validCommands =
            commands
                .Where(command => !command.IsProcessed)
                .ToList();

        try {
            foreach (var startingNode in validCommands.Select(command => command.Path.First())) {
                FindCycle(startingNode, validCommands, []);
            }
        }
        catch (CycleFound exception) {
            new Skirmish().Resolve(exception.Cycle, world);
            return true;
        }

        return false;
    }

    private static bool ResolveInvasion(List<MoveUnit> commands, World world)
    {
        var isResolutionDone = false;
        
        commands
            .Where(command => !command.IsProcessed)
            .GroupBy(command => command.Path.Second())
            .Where(group => group.DistinctBy(command => command.Issuer).Count() == 1)
            .Where(group => group.First().Issuer != group.First().Path.Second().Owner)
            .Each(group =>
            {
                new Invasion().Resolve(group.ToList(), world);
                isResolutionDone = true;
            });

        return isResolutionDone;
    }


    private static void FindCycle(Territory currentNode, List<MoveUnit> edges, List<MoveUnit> stack)
    {
        if (stack.Count == 0) {
            foreach (var move in edges.Where(move => move.Path.First() == currentNode)) {
                FindCycle(move.Path.Second(), edges, [move]);
            }
        }

        else if (currentNode == stack.First().Path.First()) {
            throw new CycleFound(stack);
        }

        else {
            foreach (var move in edges.Where(move => move.Path.First() == currentNode)) {
                List<MoveUnit> newStack = [..stack];
                newStack.Add(move);
                FindCycle(move.Path.Second(), edges, newStack);
            }
        }
    }

    private class CycleFound(List<MoveUnit> cycle) : Exception
    {
        public readonly List<MoveUnit> Cycle = cycle;
    }
}