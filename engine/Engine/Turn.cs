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
        Phase.Final,
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
            Phase.Final => ProcessFinalPhase,
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

        commands
            .OfType<SpawnArmy>()
            .Tap(spawnArmies => ValidateAndProcess(spawnArmies, true));
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

    private void ProcessFinalPhase(List<Command> commands)
    {
        // Bivouac
        World.Territories.Values
            .Where(territory => territory.Constructs.Contains(Construct.Bivouac))
            .Where(territory => !territory.IsWasteland)
            .Where(territory => !territory.IsNeutral)
            .Each(territory => territory.Units.AddArmy());
        
        // Increase Loyalty
        World.Territories.Values
            .Where(territory => !territory.IsNeutral)
            .Each(territory => territory.Loyalty++);
        
        // Add IP per territory
        World.Territories.Values
            .Where(territory => !territory.IsNeutral)
            .GroupBy(territory => territory.Owner)
            .Each(group => group.Key!.InfluencePoints += group.Count());
        
        // Add IP per HQ
        World.Territories.Values
            .Where(territory => !territory.IsNeutral)
            .Where(territory => territory.ContainsHq)
            .Each(territory => territory.Owner!.InfluencePoints += 6);

        // Add IP per 10 Armies
        World.Territories.Values
            .Where(territory => !territory.IsNeutral)
            .GroupBy(territory => territory.Owner)
            .Each(group => group.Key!.InfluencePoints += group.Sum(territory => territory.Units.Armies) / 10);
    }
}