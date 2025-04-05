using engine.Models;

namespace engine.Engine.Commands;

public abstract class Command : IProcessable
{
    public readonly List<(RejectReason Reason, IEnumerable<Command> Conflicts)> Rejections = [];

    /// <summary>
    ///     If the Command is Forced, then it will be excluded from validation.
    ///     Note that this may influence the validation of other, related Commands.
    /// </summary>
    public bool Force = false;

    public required Player Issuer;
    public bool IsRejected => !Force && Rejections.Count > 0;
    public abstract void Process(World world);
    public abstract Phase Phase();

    protected void Reject(RejectReason reason, IEnumerable<Command>? conflictingCommands = null)
    {
        Rejections.Add((reason, conflictingCommands ?? []));
    }
}