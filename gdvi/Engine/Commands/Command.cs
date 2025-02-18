using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class Command
{
    public abstract Phase Phase();
    public abstract void Process(World world);

    public required Player Issuer;

    public readonly List<(CommandRejection Reason, IEnumerable<Command> Conflicts)> Rejections = [];
    public bool IsRejected => Rejections.Count > 0;
    
    public static void Validate<T>(IEnumerable<T> commands, World world) where T : Command
    {
    }

    protected void Reject(CommandRejection reason, IEnumerable<Command> conflictingCommands)
    {
        Rejections.Add((reason, conflictingCommands));
    }
}
