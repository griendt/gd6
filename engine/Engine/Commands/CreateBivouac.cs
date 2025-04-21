using engine.Models;

namespace engine.Engine.Commands;

public class CreateBivouac : CreateConstructCommand
{
    protected override Construct ConstructType() => Construct.Bivouac;
    protected override int Cost(World world) => 20 + 10 * world.Territories.Values.Count(territory => territory.Owner == Issuer && territory.Constructs.Contains(Construct.Bivouac));
}