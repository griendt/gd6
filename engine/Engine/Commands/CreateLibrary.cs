using engine.Models;

namespace engine.Engine.Commands;

public class CreateLibrary : CreateConstructCommand
{
    protected override Construct ConstructType() => Construct.Library;
    protected override int Cost(World world) => 10 + 10 * world.Territories.Values.Count(territory => territory.Owner == Issuer && territory.Constructs.Contains(Construct.Library));
}