using engine.Models;

namespace engine.Engine.Commands;

/// <summary>
/// `CreateMine` is considered a construct command for the sake of validations
/// (on owner origin, but more importantly, IP). However, mines are not considered
/// Constructs in the sense that mines may co-exist with another construct, and
/// that a territory can contain multiple mines.
/// </summary>
public class CreateMine : CreateConstructCommand
{
    protected override Construct ConstructType() => Construct.Mine;
    protected override int Cost(World world) => 5;
}