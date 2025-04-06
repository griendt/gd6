using engine.Engine.Commands;

namespace engine.Engine;

public static class MoveArmyOrderResolver
{
    /// <summary>
    ///     Executes one step of resolution. Returns `true` if at least one
    ///     move has been resolved, `false` otherwise. Note that this method
    ///     can and will mutate the commands in-place.
    /// </summary>
    /// <param name="commands">The commands to process.</param>
    /// <returns>Whether any resolution has been done.</returns>
    public static bool Resolve(List<MoveArmy> commands)
    {
        return false;
    }
}