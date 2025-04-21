using engine.Models;

namespace engine.Engine.Commands;

public class CreateFortress : CreateConstructCommand
{
    protected override Construct ConstructType() => Construct.Fortress;
    protected override int Cost(World world) => 20;
}