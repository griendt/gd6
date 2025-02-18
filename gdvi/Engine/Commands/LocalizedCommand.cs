using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class LocalizedCommand : Command
{
    public required Territory Origin;
}