using gdvi.Engine.Commands;
using gdvi.Models;

namespace gdvi.Engine;

public class Turn(World world)
{
    private static readonly Phase[] Phases = [Phase.Natural, Phase.Inventory, Phase.Construction, Phase.Movement, Phase.Final];
    public List<Command> Commands = [];

    public void Process()
    {
        foreach (var phase in Phases) {
            ProcessPhase(phase);
        }
    }

    private void ProcessPhase(Phase phase)
    {
        var commands = Commands
            .Where(command => command.Phase() == phase)
            .ToList();
        
        if (phase == Phase.Natural) {
            ProcessNaturalPhase(commands);
        }
        else if (phase == Phase.Construction) {
            ProcessConstructionPhase(commands);
        }
    }

    private void ProcessConstructionPhase(List<Command> commands)
    {
        var createHqs = commands.Where(command => command is CreateHq).ToList();

        CreateHq.Validate(createHqs, world);
        var x = 3;
    }

    private void ProcessNaturalPhase(List<Command> commands)
    {
        foreach (var territory in world.Territories.Values) {
            territory.ApplyWastelandPenalty();
        }
    }
}