using System.Text.RegularExpressions;
using engine.Engine.Commands;
using engine.Models;
using gdl.Exceptions;

namespace gdl;

public class CommandType
{
    private CommandType(string value)
    {
        Value = value;
    }

    private string Value { get; }

    public static CommandType Set => new("Set");
    public static CommandType Con => new("Con");
    public static CommandType Inv => new("Inv");
    public static CommandType Mov => new("Mov");

    public override string ToString()
    {
        return Value;
    }
}

public partial class GdlParser(World world)
{
    public List<Command> Commands = [];
    private Player? CurrentIssuer;

    public void Parse(string lines)
    {
        using var reader = new StringReader(lines);

        while (true) {
            var line = reader.ReadLine();
            if (line == null) {
                break;
            }

            var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            if (CurrentIssuer == null && parts[0] != "Set") {
                throw new CommandSetNotInitialized();
            }

            Action<string[]> callback = parts[0] switch
            {
                "Set" => InitializeMoveset,
                "Con" => Construct,
                _ => throw new ArgumentOutOfRangeException(),
            };

            callback(parts);
        }
    }

    private void InitializeMoveset(string[] command)
    {
        try {
            CurrentIssuer = world.Players.First(player => player.Name == command[1]);
        }
        catch (InvalidOperationException) {
            throw new UnknownPlayerException();
        }
    }

    private void Construct(string[] command)
    {
        if (command.Length != 3) {
            throw new Exception("Construction instruction needs exactly 2 arguments");
        }

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

        if (command[2] == "Hq") {
            Commands.Add(new CreateHq { Issuer = CurrentIssuer!, Origin = target });
        }

        else if (!ArmiesRegex().Match(command[2]).Success) {
            throw new Exception("Could not parse second argument");
        }
    }

    [GeneratedRegex(@"^\d+A$")]
    private static partial Regex ArmiesRegex();
}