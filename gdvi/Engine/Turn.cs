using gdvi.Engine.Commands;
using gdvi.Models;

namespace gdvi.Engine;

public class Turn(World world)
{
    private static readonly Phase[] Phases = [Phase.Natural, Phase.Construction];
    public readonly List<Command> Commands = [];

    public void Process()
    {
        foreach (var phase in Phases) {
            ProcessPhase(phase);
        }
    }

    private void ProcessPhase(Phase phase)
    {
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

        CommandValidator.Validate(createHqs, world);
        // TODO: execute valid commands
    }

    private void ProcessNaturalPhase(List<Command> commands)
    {
        foreach (var territory in world.Territories.Values) {
            territory.ApplyWastelandPenalty();
        }
    }
}