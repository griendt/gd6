using gdvi.Models;

namespace gdvi.Engine.Commands;

public abstract class Command
{
    public abstract Phase Phase();
    public abstract void Process(World world);

    public required Player Issuer;

    public readonly List<(RejectReason Reason, IEnumerable<Command> Conflicts)> Rejections = [];
    public bool IsRejected => Rejections.Count > 0;

    protected void Reject(RejectReason reason, IEnumerable<Command>? conflictingCommands = null)
    {
        Rejections.Add((reason, conflictingCommands ?? []));
    }
}
