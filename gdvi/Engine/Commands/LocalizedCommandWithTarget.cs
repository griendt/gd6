using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class LocalizedCommandWithTarget: LocalizedCommand
{
    public required Territory Target;
}