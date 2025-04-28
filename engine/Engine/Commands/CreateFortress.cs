using engine.Models;

namespace engine.Engine.Commands;

public class CreateWatchtower : CreateConstructCommand
{
    protected override Construct ConstructType() => Construct.Watchtower;
    protected override int Cost(World world) => 20;
}