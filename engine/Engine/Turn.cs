using engine.Engine.Commands;
using engine.Models;

namespace engine.Engine;

public class Turn
{
    private static readonly Phase[] Phases =
    [
        Phase.Natural,
        Phase.Construction,
        Phase.Inventory,
        Phase.Movement,
    ];

    private bool _abort;
    public required List<Command> Commands = [];
    public required World World;

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

        var commands1 = Commands
            .Where(command => command.Phase() == phase)
            .ToList();

        callback(commands1);
    }

    private void ValidateAndProcess(IEnumerable<Command> commands, bool abortOnRejections = false)
    {
        commands
            .Tap(commandList => CommandValidator.Validate(commandList, World))
            .Each(command =>
            {
                if (command.IsRejected) {
                    if (abortOnRejections) {
                        _abort = true;
                    }
                }
                else {
                    command.Process(World);
                }
            });
    }

    private void ProcessNaturalPhase(List<Command> commands)
    {
        foreach (var territory in World.Territories.Values) {
            territory.ApplyWastelandPenalty();
        }
    }

    private void ProcessConstructionPhase(List<Command> commands)
    {
        commands
            .OfType<CreateHq>()
            .Tap(createHqs => ValidateAndProcess(createHqs, true));
    }

    private void ProcessInventoryPhase(List<Command> commands)
    {
        commands
            .OfType<UseDynamite>()
            .Tap(useDynamites => ValidateAndProcess(useDynamites));

        commands
            .OfType<UseCropSupply>()
            .Tap(useCropSupply => ValidateAndProcess(useCropSupply));
        
        commands
            .OfType<UseToxicWaste>()
            .Tap(useToxicWastes => ValidateAndProcess(useToxicWastes));
        
        // It is important that BuyItem occur after item usage        
        commands
            .OfType<BuyItemCommand>()
            .Tap(buyItems => ValidateAndProcess(buyItems));
    }

    private void ProcessMovementPhase(List<Command> commands)
    {
        var validMoves = commands
            .OfType<MoveArmy>()
            .Tap(moveArmies => CommandValidator.Validate(moveArmies, World))
            .Where(moveArmy => !moveArmy.IsRejected)
            .ToList();

        // Resolve until no more resolution is done
        while (MoveArmyOrderResolver.Resolve(validMoves, World)) {
        }
    }
}