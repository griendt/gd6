using gdvi.Models;

namespace gdvi.Engine.Commands;

public class SpawnArmy : LocalizedCommand
{
    public override Phase Phase() => Engine.Phase.Construction;
    
    public required int Quantity;
    
    public override void Process(World world)
    {
        world.Territories[Origin.Id].Units.AddArmies(Quantity);
    }

}