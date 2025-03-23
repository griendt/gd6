using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine;

public class Turn(World world)
{
    private static readonly Phase[] Phases =
    [
        Phase.Natural,
        Phase.Construction,
        Phase.Inventory,
        Phase.Movement,
    ];

    private readonly List<Command> Commands = [];

    private bool _abort;

    public void Process()
    {
        foreach (var phase in Phases) {
            ProcessPhase(phase);

            if (_abort) {
                break;
            }
        }
    }

    private void ProcessPhase(Phase phase)
    {
        Action<List<Command>> callback = phase switch
        {
            Phase.Natural => ProcessNaturalPhase,
            Phase.Construction => ProcessConstructionPhase,
            Phase.Inventory => ProcessInventoryPhase,
            Phase.Movement => ProcessMovementPhase,
            _ => throw new ArgumentOutOfRangeException(nameof(phase), "Unknown phase"),
        };

        var commands = Commands
            .Where(command => command.Phase() == phase)
            .ToList();

        callback(commands);
    }

    private void ValidateAndProcess(List<Command> commands, bool abortOnRejections = false)
    {
        commands
            .Tap(commandList => CommandValidator.Validate(commandList, world))
            .Where(command => !command.IsRejected)
            .Each(command => command.Process(world));

        if (abortOnRejections && commands.Find(command => command.IsRejected) != null) {
            _abort = true;
        }
    }

    private void ProcessNaturalPhase(List<Command> commands)
    {
        foreach (var territory in world.Territories.Values) {
            territory.ApplyWastelandPenalty();
        }
    }

    private void ProcessConstructionPhase(List<Command> commands)
    {
        commands
            .Where(command => command is CreateHq)
            .ToList()
            .Tap(createHqs => ValidateAndProcess(createHqs, true));
    }

    private void ProcessInventoryPhase(List<Command> commands)
    {
        commands
            .Where(command => command is UseDynamite)
            .ToList()
            .Tap(useDynamites => ValidateAndProcess(useDynamites));

        commands
            .Where(command => command is UseCropSupply)
            .ToList()
            .Tap(useCropSupply => ValidateAndProcess(useCropSupply));
    }

    private void ProcessMovementPhase(List<Command> commands)
    {
        commands
            .Where(command => command is MoveArmy)
            .ToList()
            .Tap(moveArmies => ValidateAndProcess(moveArmies));
    }
}