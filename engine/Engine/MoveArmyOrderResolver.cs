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
    public static bool Resolve(List<MoveArmy> commands, World world) =>
        ResolveBasicSkirmish(commands, world)
        || ResolveCircularInvasion(commands, world);

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

    private static bool ResolveCircularInvasion(List<MoveArmy> commands, World world)
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

    private static void FindCycle(Territory currentNode, List<MoveArmy> edges, List<MoveArmy> stack)
    {
        if (stack.Count == 0) {
            foreach (var move in edges.Where(move => move.Path.First() == currentNode)) {
                FindCycle(move.Path.Skip(1).First(), edges, [move]);
            }
        }

        else if (currentNode == stack.First().Path.First()) {
            throw new CycleFound(stack);
        }

        else {
            foreach (var move in edges.Where(move => move.Path.First() == currentNode)) {
                List<MoveArmy> newStack = [..stack];
                newStack.Add(move);
                FindCycle(move.Path.Skip(1).First(), edges, newStack);
            }
        }
    }

    private class CycleFound(List<MoveArmy> cycle) : Exception
    {
        public readonly List<MoveArmy> Cycle = cycle;
    }
}