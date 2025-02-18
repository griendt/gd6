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
        // TODO: use method attributes!
        Action<List<Command>> callback = phase switch
        {
            Phase.Natural => ProcessNaturalPhase,
            Phase.Construction => ProcessConstructionPhase,
            _ => throw new ArgumentOutOfRangeException(nameof(phase), "Unknown phase"),
        };

        var commands = Commands
            .Where(command => command.Phase() == phase)
            .ToList();

        callback(commands);
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