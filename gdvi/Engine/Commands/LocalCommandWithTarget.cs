using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class LocalCommandWithTarget: LocalCommand
{
    public required Territory Target;
}