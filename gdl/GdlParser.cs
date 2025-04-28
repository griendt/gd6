using System.Text.RegularExpressions;
using engine;
using engine.Engine;
using engine.Engine.Commands;
using engine.Models;
using gdl.Exceptions;

namespace gdl;

public partial class GdlParser(World world)
{
    private readonly Player _admin = new() { Id = Guid.NewGuid(), Name = "root" };
    public readonly List<Command> Commands = [];
    private Player? _currentIssuer;
    private bool _isInSetup;

    public IEnumerable<Turn> Turns()
    {
        var turn = new Turn { World = world, Commands = [] };

        foreach (var command in Commands) {
            if (command is EndOfTurnCommand) {
                yield return turn;
                turn = new Turn { World = world, Commands = [] };
                continue;
            }

            turn.Commands.Add(command);
        }

        // Implicit end of turn at command list end
        if (turn.Commands.Count > 0) {
            yield return turn;
        }
    }

    public void Parse(string lines)
    {
        using var reader = new StringReader(lines);

        while (true) {
            var line = reader.ReadLine();
            if (line == null) {
                break;
            }

            if (line.StartsWith("//") || line.Trim() == "") {
                continue;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (parts[0] == "InitStart") {
                _isInSetup = true;
                continue;
            }
            if (parts[0] == "InitEnd") {
                if (!_isInSetup) {
                    throw new Exception("Cannot end init if not started");
                }

                _isInSetup = false;
                continue;
            }

            if (_isInSetup) {
                Action<string[]> setup = parts[0] switch
                {
                    "AddPlayer" => AddPlayer,
                    "SetNumTerritories" => CreateTerritories,
                    "SetCoordinates" => CreateTerritoryCoordinates,
                    "SetBoundaries" => CreateTerritoryBoundaries,
                    _ => throw new UnknownCommandType(),
                };

                setup(parts);
                continue;
            }

            if (_currentIssuer == null && parts[0] != "Set") {
                throw new CommandSetNotInitialized();
            }

            Action<string[]> callback = parts[0] switch
            {
                "End" => EndOfTurn,
                "Set" => InitializeMoveSet,
                "Con" => Construct,
                "Buy" => BuyItem,
                "Use" => UseItem,
                "Mov" => Move,
                _ => throw new UnknownCommandType(),
            };

            callback(parts);
        }
    }

    private void EndOfTurn(string[] command)
    {
        Commands.Add(new EndOfTurnCommand
        {
            Issuer = _admin,
        });
    }

    private void AddPlayer(string[] command)
    {
        if (!ColorRegex().Match(command[2]).Success) {
            throw new InvalidColorException();
        }

        world.Players.Add(new Player
        {
            Id = Guid.NewGuid(),
            Name = command[1],
            Colour = command[2],
        });
    }

    private void CreateTerritories(string[] command)
    {
        Enumerable.Range(1, int.Parse(command[1]))
            .Each(i => world.Territories.Add(i, new Territory(world) { Id = i }));
    }

    private void CreateTerritoryBoundaries(string[] command)
    {
        var arguments = command[1].Split(";");

        foreach (var argument in arguments) {
            var territoryIds = argument.Split(",").Select(int.Parse).ToList();

            if (territoryIds.Count != 2) {
                throw new InvalidArgumentException();
            }

            if (!world.Territories.ContainsKey(territoryIds[0]) || !world.Territories.ContainsKey(territoryIds[1])) {
                throw new UnknownTerritoryException();
            }

            world.AddBorder(world.Territories[territoryIds[0]], world.Territories[territoryIds[1]]);
        }
    }

    private void CreateTerritoryCoordinates(string[] command)
    {
        int territoryId;

        try {
            territoryId = int.Parse(command[1]);
        }
        catch (FormatException) {
            throw new UnknownTerritoryException();
        }

        if (!world.Territories.TryGetValue(territoryId, out var territory)) {
            throw new UnknownTerritoryException();
        }

        if (!CoordinatesRegex().Match(command[2]).Success) {
            throw new InvalidArgumentException();
        }

        foreach (var coordinates in command[2].Split(";")) {
            var xy = coordinates.Split(",").Select(int.Parse).ToList();

            if (xy.Count != 2) {
                throw new InvalidArgumentException();
            }

            territory.Coordinates.Add((xy[0], xy[1]));
        }
    }

    private void InitializeMoveSet(string[] command)
    {
        try {
            _currentIssuer = world.Players.First(player => player.Name == command[1]);
        }
        catch (InvalidOperationException) {
            throw new UnknownPlayerException();
        }
    }

    private void Construct(string[] command)
    {
        int territoryId;

        try {
            territoryId = int.Parse(command[1]);
        }
        catch (FormatException) {
            throw new UnknownTerritoryException();
        }

        if (!world.Territories.ContainsKey(territoryId)) {
            throw new UnknownTerritoryException();
        }

        var target = world.Territories[territoryId];

        switch (command[2]) {
            case "Hq":
                Commands.Add(new CreateHq { Issuer = _currentIssuer!, Origin = target });
                return;
            case "Tow":
                Commands.Add(new CreateWatchtower { Issuer = _currentIssuer!, Origin = target });
                return;
            case "Biv":
                Commands.Add(new CreateBivouac { Issuer = _currentIssuer!, Origin = target });
                return;
            case "Int":
                Commands.Add(new CreateIntelligence { Issuer = _currentIssuer!, Origin = target });
                return;
        }

        var armiesMatch = ArmiesRegex().Match(command[2]);
        if (!armiesMatch.Success) {
            throw new Exception("Could not parse second argument");
        }

        Commands.Add(new SpawnArmy { Issuer = _currentIssuer!, Origin = target, Quantity = int.Parse(armiesMatch.Groups[1].Value) });
    }

    private void Move(string[] command)
    {
        var path = command[1].Split('→')
            .Select(int.Parse)
            .ToList();

        var armiesMatch = ArmiesRegex().Match(command[2]);
        if (!armiesMatch.Success) {
            throw new Exception("Could not parse second argument");
        }

        foreach (var _ in Enumerable.Range(1, int.Parse(armiesMatch.Groups[1].Value))) {
            Commands.Add(new MoveArmy
            {
                Issuer = _currentIssuer!,
                Origin = world.Territories[path[0]],
                Path = path.Select(item => world.Territories[item]).ToList(),
            });
        }
    }

    private void BuyItem(string[] command)
    {
        var itemType = command[1] switch
        {
            "Dyn" => Item.Dynamite,
            "Crp" => Item.CropSupply,
            "Tox" => Item.ToxicWaste,
            _ => throw new Exception("Unknown item type"),
        };

        Commands.Add(new BuyItemCommand
        {
            Issuer = _currentIssuer!,
            ItemType = () => itemType,
        });
    }

    private void UseItem(string[] command)
    {
        switch (command[2]) {
            case "Dyn":
                var path = command[1].Split('→')
                    .Select(int.Parse)
                    .ToList();

                if (path.Count != 2) {
                    throw new InvalidPathLengthException(2, path.Count);
                }

                Commands.Add(new UseDynamite
                {
                    Issuer = _currentIssuer!,
                    Origin = world.Territories[path[0]],
                    Target = world.Territories[path[1]],
                });
                break;
            case "Crp":
                Commands.Add(new UseCropSupply
                {
                    Issuer = _currentIssuer!,
                    Quantities = command[1].Split(',')
                        .Select(item => item.Split(':'))
                        .ToDictionary(keySelector: item => int.Parse(item[0]), elementSelector: item => int.Parse(item[1])),
                });
                break;
            default:
                throw new UnknownInventoryItemException();
        }
    }

    [GeneratedRegex(@"^(\d+)A$")]
    private static partial Regex ArmiesRegex();

    [GeneratedRegex(@"^#[\da-fA-F]{3}$")]
    private static partial Regex ColorRegex();

    [GeneratedRegex(@"^(\d+,\d+;){2,}(\d+,\d+)$")]
    private static partial Regex CoordinatesRegex();
}