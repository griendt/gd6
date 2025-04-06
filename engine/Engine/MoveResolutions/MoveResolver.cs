using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public abstract class MoveResolver
{
    public required List<MoveArmy> Moves;
    public abstract void Process(World world);
}