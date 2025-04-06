using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public abstract class MoveResolver
{
    public abstract void Resolve(List<MoveArmy> moves, World world);
}