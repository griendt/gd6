using engine.Models;

namespace engine.Engine.Commands;

public class CreateIntelligence : CreateConstructCommand
{
    protected override Construct ConstructType() => Construct.Intelligence;
    protected override int Cost(World world) => 20;
}