using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class LocalCommand : Command
{
    public required Territory Origin;
}