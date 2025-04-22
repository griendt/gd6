using System.Text.RegularExpressions;
using engine.Engine.Commands;
using engine.Models;
using gdl.Exceptions;

namespace gdl;

public partial class GdlParser(World world)
{
    public readonly List<Command> Commands = [];
    private Player? _currentIssuer;

    public void Parse(string lines)
    {
        using var reader = new StringReader(lines);

        while (true) {
            var line = reader.ReadLine();
            if (line == null) {
                break;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (_currentIssuer == null && parts[0] != "Set") {
                throw new CommandSetNotInitialized();
            }

            Action<string[]> callback = parts[0] switch
            {
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
            case "For":
                Commands.Add(new CreateFortress { Issuer = _currentIssuer!, Origin = target });
                return;
            case "Biv":
                Commands.Add(new CreateBivouac { Issuer = _currentIssuer!, Origin = target });
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
                        .ToDictionary(item => int.Parse(item[0]), item => int.Parse(item[1])),
                });
                break;
            default:
                throw new UnknownInventoryItemException();
        }
    }

    [GeneratedRegex(@"^(\d+)A$")]
    private static partial Regex ArmiesRegex();
}