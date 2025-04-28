using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine.MoveResolutions;

public abstract class MoveResolver
{
    public abstract void Resolve(List<MoveUnit> moves, World world);
}