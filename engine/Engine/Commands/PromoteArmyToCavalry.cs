using engine.Models;

namespace engine.Engine.Commands;

public class PromoteArmyToCavalry : Command, IHasOrigin
{
    public required int Quantity;
    public required Territory Origin { get; set; }
    public override Phase Phase() => Engine.Phase.Construction;

    public override void Process(World world)
    {
        world.Territories[Origin.Id].Owner = Issuer;

        foreach (var _ in Enumerable.Range(0, Quantity)) {
            Issuer.InfluencePoints -= 3;
            world.Territories[Origin.Id].Units.Pop(Unit.Army);
            world.Territories[Origin.Id].Units.AddCavalry();
        }
    }

}