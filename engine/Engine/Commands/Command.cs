using gd6.Models;

namespace gd6.Engine.Commands;

public abstract class Command
{
    public abstract Phase Phase();
    public abstract void Process(World world);

    public required Player Issuer;

    /// <summary>
    /// If the Command is Forced, then it will be excluded from validation.
    /// Note that this may influence the validation of other, related Commands.
    /// </summary>
    public bool Force = false;

    public readonly List<(RejectReason Reason, IEnumerable<Command> Conflicts)> Rejections = [];
    public bool IsRejected => !Force && Rejections.Count > 0;

    protected void Reject(RejectReason reason, IEnumerable<Command>? conflictingCommands = null)
    {
        Rejections.Add((reason, conflictingCommands ?? []));
    }
}
