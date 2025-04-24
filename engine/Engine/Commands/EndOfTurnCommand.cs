using engine.Models;

namespace engine.Engine.Commands;

public class EndOfTurnCommand : Command
{
    public sealed override Phase Phase() => Engine.Phase.Final;
    
    public override void Process(World world)
    {
    }
}